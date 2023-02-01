using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using SmartLock.Client.HttpClient;
using SmartLock.Client.Models;
using SmartLock.Client.NotificationHub;

namespace SmartLock.UI.ViewModels.Buildings;

public partial class BuildingsViewModel : ObservableObject
{
    private readonly NotificationsHubClient _notificationsHubClient;
    private readonly IBuildingsService _buildingsService;
    private readonly LockViewModelFactory _lockViewModelFactory;
    private readonly IObservable<Notification<IEvent>> _observable;

    [field: ObservableProperty]
    public ObservableCollection<LockViewModel> Locks { get; set; } = new();
    
    [field: ObservableProperty]
    public string Location { get; set; } = string.Empty;

    private Task _clientStartTask;

    public BuildingsViewModel(INotificationsHubClient notificationsHubClient, IBuildingsService buildingsService, LockViewModelFactory lockViewModelFactory, IObservable<Notification<IEvent>> observable)
    {
        _notificationsHubClient = (NotificationsHubClient)notificationsHubClient;
        _buildingsService = buildingsService;
        _lockViewModelFactory = lockViewModelFactory;
        _observable = observable;
        _clientStartTask = _notificationsHubClient.StartAsync();

        _observable.Select(x => HandleEventAsync(x).ToObservable()).Concat().Subscribe();
    }

    private async Task HandleEventAsync(INotification<IEvent> notification)
    {
        if (notification.Data is not BuildingEvent e || e.Building.Value != Location)
        {
            return;
        }

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
    private async Task LoadBuildingsAsync()
    {
        await _clientStartTask;
        var locations = await _buildingsService.GetLocksAsync(new LocationModel(Location));

        Locks.Clear();
        foreach (var location in locations)
        {
            Locks.Add(_lockViewModelFactory.Create(location));
        }

        await _notificationsHubClient.SubscribeAsync(Location);
    }
}