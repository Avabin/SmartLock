using BuildingsViewModel = SmartLock.UI.ViewModels.Buildings.BuildingsViewModel;

namespace SmartLock.UI;

public partial class MainPage : ContentPage
{
    public MainPage(BuildingsViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}