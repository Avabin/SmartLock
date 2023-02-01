using System.Reactive.Subjects;
using SmartLock.Client.Models;

namespace SmartLock.Client.NotificationHub;

public class ObservableNotificationReceiver : IObservable<Notification<IEvent>>, INotificationReceiver
{
    private ISubject<Notification<IEvent>> _subject = new Subject<Notification<IEvent>>();
    public IDisposable Subscribe(IObserver<Notification<IEvent>> observer) => _subject.Subscribe(observer);

    public Task ReceiveAsync(Notification<IEvent> notification)
    {
        _subject.OnNext(notification);
        return Task.CompletedTask;
    }
}