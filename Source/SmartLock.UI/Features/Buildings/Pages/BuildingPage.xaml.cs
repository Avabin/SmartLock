using CommunityToolkit.Mvvm.ComponentModel;
using SmartLock.UI.Features.Buildings.ViewModels;

namespace SmartLock.UI.Features.Buildings.Pages;

[QueryProperty(nameof(BuildingViewModel), nameof(BuildingViewModel))]
public partial class BuildingPage : ContentPage
{
    [field: ObservableProperty] public BuildingViewModel? BuildingViewModel { get; set; } = null;
    public BuildingPage()
    {
        InitializeComponent();
        
        this.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(BuildingViewModel))
            {
                BindingContext = BuildingViewModel;
            }
        };
    }
}