using CommunityToolkit.Mvvm.ComponentModel;
using SmartLock.UI.Features.Notifications;
using SmartLock.UI.ViewModels;

namespace SmartLock.UI;

public partial class AppShell : Shell
{
    public AppShell(ShellViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}

public partial class ShellViewModel : ObservableObject
{
    [ObservableProperty]
    private NotificationsStatusViewModel _statusViewModel;

    public ShellViewModel(NotificationsStatusViewModel statusViewModel)
    {
        _statusViewModel = statusViewModel;
    }
}