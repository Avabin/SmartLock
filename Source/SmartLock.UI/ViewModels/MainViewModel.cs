using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLock.UI.Features.Notifications;
using SmartLock.UI.Features.Settings;

namespace SmartLock.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private NotificationsStatusViewModel _statusViewModel;

    public MainViewModel(NotificationsStatusViewModel statusViewModel)
    {
        _statusViewModel = statusViewModel;
    }
}