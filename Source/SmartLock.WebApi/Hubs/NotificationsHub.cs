using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using SmartLock.Client;
using SmartLock.Client.NotificationHub;
using SmartLock.Orleans.Core;

namespace SmartLock.WebApi.Hubs;

public class NotificationsHub : Hub<INotificationsHubClient>
{
    private readonly ILogger<NotificationsHub> _logger;
    private readonly NotificationsHubService _service;

    public NotificationsHub(ILogger<NotificationsHub> logger, NotificationsHubService service)
    {
        _logger = logger;
        _service = service;
    }
    
    public async Task SubscribeAsync(string ns, string streamId)
    {

        await _service.SubscribeAsync(ns, streamId, Context.ConnectionId);
    }

    public async Task SubscribeNotificationsAsync(string streamId) => 
        await SubscribeAsync(StreamProviderConstants.NotificationsNamespace, streamId);


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Disconnected");
        _logger.LogDebug("ConnectionId: {ConnectionId}", Context.ConnectionId);
        // remove connection from groups
        var streams = _service.Subscriptions.Keys.ToArray();
        _logger.LogDebug("Removing connection from {Count} groups", streams.Length);
        await Task.WhenAll(streams.Select(stream => Groups.RemoveFromGroupAsync(Context.ConnectionId, stream.ToString())));
        // remove connection from connections
        _service.Connections.TryRemove(Context.ConnectionId, out _);
        // unsubscribe from streams
        var handles = streams.Select(stream => _service.Subscriptions[stream]).ToArray();
        _logger.LogDebug("Unsubscribing from {Count} streams", handles.Length);
        await Task.WhenAll(handles.Select(handle => handle.UnsubscribeAsync()));
        await base.OnDisconnectedAsync(exception);

    }
}