namespace SmartLock.Client.NotificationHub;

public interface INotificationsHubClient
{
    const string HubPath = "/hubs/notifications";
    Task OnNextAsync(string notification);
}