using SmartLock.UI.ViewModels;

namespace SmartLock.UI.Pages;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}