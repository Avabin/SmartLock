using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Orleans.Runtime;
using Orleans.Streams;
using SmartLock.Client;
using SmartLock.Client.Models;
using SmartLock.Client.NotificationHub;
using SmartLock.Orleans.Core;

namespace SmartLock.WebApi.Hubs;

public class NotificationsHubService
{
    private readonly IClusterClient _client;
    private readonly ILogger<NotificationsHubService> _logger;
    private readonly IHubContext<NotificationsHub, INotificationsHubClient> _hubContext;
    private readonly IStreamProvider _streamProvider;


    private static readonly ActivitySource ActivitySource = new($"SmartLock.WebApi.{nameof(NotificationsHubService)}");
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    public ConcurrentDictionary<StreamId, StreamSubscriptionHandle<Notification<IEvent>>> Subscriptions { get; } =
        new();

    public ConcurrentDictionary<string, ConcurrentBag<StreamId>> Connections { get; } = new();

    public NotificationsHubService(IClusterClient client, ILogger<NotificationsHubService> logger,
        IHubContext<NotificationsHub, INotificationsHubClient> hubContext)
    {
        _client = client;
        _logger = logger;
        _hubContext = hubContext;
        _streamProvider = _client.GetStreamProvider(StreamProviderConstants.DefaultStreamProviderName);
    }

    public async Task SubscribeAsync(string ns, string streamId, string connectionId)
    {
        Connections.TryGetValue(connectionId, out var streams);
        if (streams != null && streams.Contains(StreamId.Create(ns, streamId)))
        {
            _logger.LogInformation("Client {ConnectionId} already subscribed to {Namespace}/{StreamId}", connectionId,
                ns, streamId);
            return;
        }

        _logger.LogInformation("Subscribe to {Namespace}/{StreamId}", ns, streamId);
        _logger.LogDebug("ConnectionId: {ConnectionId}", connectionId);
        var streamKey = StreamId.Create(ns, streamId);

        // try to resume subscription if it exists
        if (Subscriptions.TryGetValue(streamKey, out var h))
        {
            try
            {
                await h.ResumeAsync(async (notification, token) =>
                    await SendToClientsAsync(streamKey.ToString(), notification));
            }
            catch (InvalidOperationException e)
            {
                // h is no longer valid
                _logger.LogWarning(e, "Failed to resume subscription to {Namespace}/{StreamId}", ns, streamId);
                Subscriptions.TryRemove(streamKey, out _);
            }

            await _hubContext.Groups.AddToGroupAsync(connectionId, streamKey.ToString());
            Connections.AddOrUpdate(connectionId, new ConcurrentBag<StreamId> { streamKey }, (_, bag) =>
            {
                if (!bag.Contains(streamKey))
                    bag.Add(streamKey);
                return bag;
            });
            return;
        }

        var handle = await _streamProvider
            .GetStream<Notification<IEvent>>(streamKey)
            .SubscribeAsync(async (notification, token) =>
                await SendToClientsAsync(streamKey.ToString(), notification));

        Subscriptions.AddOrUpdate(streamKey, handle, (_, old) => handle);

        await _hubContext.Groups.AddToGroupAsync(connectionId, streamKey.ToString());
        Connections.AddOrUpdate(connectionId, new ConcurrentBag<StreamId> { streamKey }, (_, bag) =>
        {
            if (!bag.Contains(streamKey))
                bag.Add(streamKey);
            return bag;
        });
    }

    private async Task SendToClientsAsync(string streamId, INotification<IEvent> notification)
    {
        var traceState = notification.Data.TraceState;
        var traceParent = notification.Data.TraceParent;
        var activityContext = ActivityContext.Parse(traceParent, traceState);
        using var activity = ActivitySource.StartActivity("SendToClientsAsync", ActivityKind.Server, activityContext);
        activity?.SetIdFormat(ActivityIdFormat.W3C);
        activity?.AddTag("signalr.streamId", streamId);
        activity?.AddTag("rpc.service", "SmartLock.WebApi.Hubs.NotificationsHubService");
        activity?.AddTag("rpc.method", "HandleMessageAsync");
        activity?.AddTag("rpc.system", "signalr");
        activity?.AddTag("rpc.message_type", notification.Data.GetType().Name);
        var notificationsNamespace = StreamProviderConstants.NotificationsNamespace;
        activity?.AddTag("orleans.namespace", notificationsNamespace);
        activity?.AddTag("orleans.streamId", streamId);
        _logger.LogDebug("Received notification for {Namespace}/{StreamId}",
            notificationsNamespace, streamId);
        var s = JsonConvert.SerializeObject(notification, Settings);
        await _hubContext.Clients.Group(streamId).OnNextAsync(s);
    }

    public static JsonSerializerSettings Settings => new()
    {
        TypeNameHandling = TypeNameHandling.All,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
    };
}