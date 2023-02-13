using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartLock.UI.Features.Buildings;

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