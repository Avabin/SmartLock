using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLock.UI.Features.Buildings.Pages;
using SmartLock.UI.Features.Settings;

namespace SmartLock.UI.Features.Buildings.ViewModels;

public partial class BuildingsViewModel : ObservableObject, IDisposable
{
    private readonly BuildingViewModelFactory _buildingViewModelFactory;
    private readonly IClientSettingsMediator _settingsMediator;
    [field: ObservableProperty] public ObservableCollection<BuildingViewModel> Buildings { get; set; } = new();
    private IDisposable _sub;

    public BuildingsViewModel(BuildingViewModelFactory buildingViewModelFactory, IClientSettingsMediator settingsMediator)
    {
        _buildingViewModelFactory = buildingViewModelFactory;
        _settingsMediator = settingsMediator;
        
        _sub = settingsMediator.Observable
            .Select(x => x.DefaultBuilding)
            .DistinctUntilChanged()
            .Select((x) =>AddDefaultBuildingAsync().ToObservable())
            .Concat()
            .Subscribe();
    }
    
    [field: ObservableProperty] public string Location { get; set; } = string.Empty;
    [RelayCommand]
    private async Task AddDefaultBuildingAsync()
    {
        var defaultBuilding = await _settingsMediator.Observable.Select(x => x.DefaultBuilding).FirstAsync();
        
        if(Buildings.Any(x => x.Location == defaultBuilding))
        {
            return;
        }

        if (defaultBuilding is null || defaultBuilding.Value == string.Empty)
        {
            return;
        }
        
        var building = _buildingViewModelFactory.Create(defaultBuilding.Value);
        
        await building.LoadCommand.ExecuteAsync(null);
        
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Buildings.Add(building);
        });
    }
    
    [RelayCommand]
    private async Task AddBuildingAsync()
    {
        if(Buildings.Any(x => x.Location == Location))
        {
            return;
        }
        var building = _buildingViewModelFactory.Create(Location);

        await building.LoadCommand.ExecuteAsync(null);
        
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Buildings.Add(building);
        });
    }
    
    [RelayCommand]
    private async Task GoToBuildingAsync(BuildingViewModel building) =>
        await Shell.Current.GoToAsync(nameof(BuildingPage), new Dictionary<string, object>()
        {
            {nameof(BuildingViewModel), building}
        });

    public void Dispose()
    {
        _sub.Dispose();
    }
}