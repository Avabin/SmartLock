using CommunityToolkit.Mvvm.ComponentModel;
using SmartLock.Client.Models;
using IEvent = SmartLock.Client.Models.IEvent;

namespace SmartLock.UI.Features.Buildings.Locks;

public partial class LockViewModel : ObservableObject
{
    private readonly IObservable<Notification<IEvent>> _observable;
    [ObservableProperty] private string _location;
    [ObservableProperty]
    private bool _isLocked = true;

    [ObservableProperty] private Color _lockColor;

    public LockViewModel(LockModel status, IObservable<Notification<IEvent>> observable)
    {
        _observable = observable;
        Location = status.Location.Value;
        IsLocked = !status.IsOpen;

        this.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName is nameof(IsLocked))
            {
                this.LockColor = IsLocked ? Colors.Red : Colors.Green;
            }
        };

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