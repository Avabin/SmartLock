using SmartLock.Client.Models;

namespace SmartLock.Client.NotificationHub;

public interface INotificationReceiver
{
    Task ReceiveAsync(Notification<IEvent> notification);
}