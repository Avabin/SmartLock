using MemoryPack;

namespace SmartLock.Client.Models;

[GenerateSerializer, Immutable, MemoryPackable]
public partial record Notification<T>([Immutable] [property: Id(0)]T Data, [Immutable] [property: Id(1)] DateTimeOffset Timestamp) : INotification<T> where T : IEvent
{
};

public static class Notification
{
    public static Notification<T> Of<T>(T @event, DateTimeOffset? timestamp = null) where T : IEvent => new(@event, timestamp ?? DateTimeOffset.UtcNow);
}