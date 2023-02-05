using System.Data;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Polly;
using SmartLock.Client.Models;

namespace SmartLock.Client.NotificationHub;

public class NotificationsHubClient : INotificationsHubClient
{
    private readonly IEnumerable<INotificationReceiver> _receivers;
    private HubConnection _hubConnection;
    private ISubject<ConnectionStatus> _connectionState = new ReplaySubject<ConnectionStatus>(1);
    public IObservable<ConnectionStatus> ConnectionState => _connectionState.AsObservable();
    private IAsyncPolicy _policy = Policy.Handle<Exception>()
        .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(Math.Min(Math.Pow(2, retryAttempt), 30)));

    public NotificationsHubClient(string baseUrl, IEnumerable<INotificationReceiver> receivers)
    {
        _connectionState.OnNext(ConnectionStatus.Disconnected);
        _receivers = receivers;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{baseUrl}{INotificationsHubClient.HubPath}")
            .Build();
        _hubConnection.On<string>("OnNextAsync", OnNextAsync);
        
        _hubConnection.Closed += async (error) =>
        {
            _connectionState.OnNext(ConnectionStatus.Disconnected);
            await Task.Delay(Random.Shared.Next(0, 5) * 1000);
            _connectionState.OnNext(ConnectionStatus.Connecting);
            await _hubConnection.StartAsync();
        };

        _connectionState.OnNext(ConnectionStatus.Created);
    }
    
    public async Task StartAsync() => 
        await _policy.ExecuteAsync(StartAsyncInner);

    private async Task StartAsyncInner()
    {
        
        _connectionState.OnNext(ConnectionStatus.Connecting);
        try
        {
            await _hubConnection.StartAsync();
            _connectionState.OnNext(ConnectionStatus.Connected);
        }
        catch (Exception e)
        {
            _connectionState.OnNext(ConnectionStatus.Disconnected);
            throw;
        }
    }

    public async Task SubscribeAsync(string key)
    {
        await _hubConnection.InvokeAsync("SubscribeNotificationsAsync",  key);
    }
    public async Task OnNextAsync(string notification)
    {
        var notif = JsonConvert.DeserializeObject<Notification<IEvent>>(notification, Settings);
        if(notif is null)
            return;
        await Task.WhenAll(_receivers.Select(r => r.ReceiveAsync(notif)));
    }
    
    
    public static JsonSerializerSettings Settings => new()
    {
        TypeNameHandling = TypeNameHandling.All,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
    };
}

public enum ConnectionStatus
{
    Unknown,
    Created,
    Connecting,
    Connected,
    Disconnected
}