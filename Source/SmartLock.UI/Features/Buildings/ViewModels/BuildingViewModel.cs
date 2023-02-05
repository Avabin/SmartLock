using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLock.Client.HttpClient;
using SmartLock.Client.Models;
using SmartLock.Client.NotificationHub;

namespace SmartLock.UI.Features.Buildings.ViewModels;

public partial class BuildingViewModel : ObservableObject, IDisposable
{
    private readonly NotificationsHubClient _notificationsHubClient;
    private readonly IBuildingsService _buildingsService;
    private readonly LockViewModelFactory _lockViewModelFactory;
    private readonly IObservable<Client.Models.Notification<IEvent>> _observable;

    [field: ObservableProperty]
    public ObservableCollection<LockViewModel> Locks { get; set; } = new();
    
    public ObservableCollection<LockViewModel> SortedLocks {get; set;} = new();
    
    [field: ObservableProperty]
    public string Location { get; set; } = string.Empty;
    
    [field: ObservableProperty] public Color SignalColor { get; set; } = Colors.DarkViolet;
    
    private ISubject<Unit> _changing = new Subject<Unit>();
    public IObservable<Unit> Changing => _changing.AsObservable();
    private CompositeDisposable _disposables = new();

    public BuildingViewModel(INotificationsHubClient notificationsHubClient, IBuildingsService buildingsService, LockViewModelFactory lockViewModelFactory, IObservable<Client.Models.Notification<IEvent>> observable)
    {
        _notificationsHubClient = (NotificationsHubClient)notificationsHubClient;
        _buildingsService = buildingsService;
        _lockViewModelFactory = lockViewModelFactory;
        _observable = observable;

        var d = (IDisposable d) => _disposables.Add(d);
        d(_observable.Select(x => HandleEventAsync(x).ToObservable()).Concat().Subscribe());
        
        d(Changing.Select(x => BlinkAsync().ToObservable()).Concat().Subscribe());
    }

    private async Task BlinkAsync()
    {
        var oldColor = SignalColor;
        SignalColor = Colors.Red;
        await Task.Delay(250).ConfigureAwait(false);
        SignalColor = Colors.White;
        await Task.Delay(250).ConfigureAwait(false);
        SignalColor = oldColor;
    }

    private async Task HandleEventAsync(INotification<IEvent> notification)
    {
        if (notification.Data is not BuildingEvent e || e.Building.Value != Location)
        {
            return;
        }

        _changing.OnNext(Unit.Default);
        switch (notification.Data)
        {
            case AddLock addLockEvent:
                var @lock = await _buildingsService.GetLockAsync(addLockEvent.Lock, new LocationModel(Location));
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Locks.Add(_lockViewModelFactory.Create(@lock));
                });
                break;
            case RemoveLock removeLockEvent:
                var lockToRemove =
                    Locks.FirstOrDefault(lockViewModel => lockViewModel.Location == removeLockEvent.Lock.Value);
                if (lockToRemove != null)
                {
                    MainThread.BeginInvokeOnMainThread(() => Locks.Remove(lockToRemove));
                }

                break;
        }
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        var locations = await _buildingsService.GetLocksAsync(new LocationModel(Location));

        Locks.Clear();
        foreach (var location in locations)
        {
            Locks.Add(_lockViewModelFactory.Create(location));
        }
        SortedLocks.Clear();
        foreach (var lockViewModel in Locks.OrderBy(x => x.Location))
        {
            SortedLocks.Add(lockViewModel);
        }

        await _notificationsHubClient.SubscribeAsync(Location);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}