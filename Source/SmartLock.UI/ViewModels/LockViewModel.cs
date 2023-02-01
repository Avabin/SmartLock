using System.ComponentModel;
using SmartLock.Client.Models;
using SmartLock.UI.ViewModels.Buildings;
using IEvent = SmartLock.Client.Models.IEvent;

namespace SmartLock.UI.ViewModels;

public partial class LockViewModel : INotifyPropertyChanged
{
    private readonly IObservable<Notification<IEvent>> _observable;
    public string Location { get; set; }
    public bool IsLocked { get; set; } = true;
    public Color Color => IsLocked ? Colors.Red : Colors.Green;

    public LockViewModel(LockModel status, IObservable<Notification<IEvent>> observable)
    {
        _observable = observable;
        Location = status.Location.Value;
        IsLocked = !status.IsOpen;

        _observable.Subscribe(notification =>
        {
            switch (notification.Data)
            {
                case OpenLock @event:
                    if (@event.Location.Value == Location)
                    {
                        IsLocked = false;
                    }

                    break;
                case CloseLock @event:
                    if (@event.Location.Value == Location)
                    {
                        IsLocked = true;
                    }

                    break;
                case OpenAllLocks _:
                    IsLocked = false;
                    break;
                case CloseAllLocks _:
                    IsLocked = true;
                    break;
            }
        });
    }
}