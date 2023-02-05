using SmartLock.UI.Features.Buildings.ViewModels;
using SmartLock.UI.Features.Buildings.Views;

namespace SmartLock.UI.Features.Buildings.Pages;

public partial class BuildingsPage : ContentPage
{
    public BuildingsPage(BuildingsViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }

    private void TapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
    {
        if (BindingContext is not BuildingsViewModel)
        {
            return;
        }

        if (sender is not BuildingView{BindingContext: BuildingViewModel vm})
        {
            return;
        }
        
        Shell.Current.GoToAsync(nameof(BuildingPage), new Dictionary<string, object>()
        {
            {nameof(BuildingViewModel), vm}
        });
    }
}