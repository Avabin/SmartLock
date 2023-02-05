using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLock.Client.HttpClient;
using SmartLock.Client.Models;

namespace SmartLock.UI.Features.Settings;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IClientSettingsService _settingsService;
    private readonly IClientSettingsMediator _settingsMediator;

    [ObservableProperty]
    private string _deviceId;

    [ObservableProperty]
    private string _name;
    
    [ObservableProperty]
    private string _defaultBuilding;

    public SettingsViewModel(IClientSettingsService settingsService, IDeviceIdService deviceIdService, IClientSettingsMediator settingsMediator)
    {
        _settingsService = settingsService;
        _settingsMediator = settingsMediator;
        DeviceId = deviceIdService.GetDeviceId();
        Name = "SmartLock.Client";
        DefaultBuilding = "";

        _ = LoadSettingsCommand.ExecuteAsync(null);
    }

    private void CopyFrom(ref ClientSettings settings)
    {
        DefaultBuilding = settings.DefaultBuilding;
        _settingsMediator.Publish(settings);
    }
    
    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        var settings = new ClientSettings
        {
            DeviceId = DeviceId,
            Name = Name,
            DefaultBuilding = DefaultBuilding
        };
        await _settingsService.RegisterAsync(DeviceId, Name, DefaultBuilding);
        _settingsMediator.Publish(settings);
    }
    
    [RelayCommand]
    private async Task LoadSettingsAsync()
    {
        var settings = await _settingsService.GetClientSettingsAsync(DeviceId);
        CopyFrom(ref settings);
    }
}