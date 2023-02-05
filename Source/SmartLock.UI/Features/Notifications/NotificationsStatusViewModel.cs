using CommunityToolkit.Mvvm.ComponentModel;
using SmartLock.Client.NotificationHub;

namespace SmartLock.UI.Features.Notifications;

public class NotificationsStatusViewModel : ObservableObject, IDisposable
{
    private readonly NotificationsHubClient _hubClient;
    
    [field: ObservableProperty] public ConnectionStatus ConnectionStatus { get; set; } = ConnectionStatus.Disconnected;

    private IDisposable _sub;

    public NotificationsStatusViewModel(INotificationsHubClient hubClient)
    {
        _hubClient = (NotificationsHubClient) hubClient;

        _sub = _hubClient.ConnectionState.Subscribe(status => ConnectionStatus = status);
    }

    public void Dispose()
    {
        _sub.Dispose();
    }
}