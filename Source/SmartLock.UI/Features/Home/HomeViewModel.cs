using CommunityToolkit.Mvvm.ComponentModel;
using SmartLock.UI.Features.Notifications;

namespace SmartLock.UI.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty]
    private NotificationsStatusViewModel _statusViewModel;

    public HomeViewModel(NotificationsStatusViewModel statusViewModel)
    {
        _statusViewModel = statusViewModel;
    }
}