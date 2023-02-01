using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using SmartLock.Client.Models;

namespace SmartLock.Client.NotificationHub;

public class NotificationsHubClient : INotificationsHubClient
{
    private readonly IEnumerable<INotificationReceiver> _receivers;
    private HubConnection _hubConnection;

    public NotificationsHubClient(string baseUrl, IEnumerable<INotificationReceiver> receivers)
    {
        _receivers = receivers;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{baseUrl}{INotificationsHubClient.HubPath}")
            .Build();
        _hubConnection.On<string>("OnNextAsync", OnNextAsync);
        
        _hubConnection.Closed += async (error) =>
        {
            await Task.Delay(Random.Shared.Next(0, 5) * 1000);
            await _hubConnection.StartAsync();
        };

    }
    
    public async Task StartAsync()
    {
        await _hubConnection.StartAsync();
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