using SmartLock.UI.ViewModels;

namespace SmartLock.UI.Pages;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}