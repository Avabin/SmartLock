namespace SmartLock.Client.Models;

public interface INotification<out T> where T : IEvent
{
    T Data { get; }
    DateTimeOffset Timestamp { get; }
}