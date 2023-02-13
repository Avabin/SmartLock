using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using SmartLock.Client;
using SmartLock.Client.Models;
using SmartLock.UI.Features.Buildings;
using SmartLock.UI.Features.Buildings.Locks;
using SmartLock.UI.Features.Detection.Pages;
using SmartLock.UI.Features.Detection.ViewModels;
using SmartLock.UI.Features.Notifications;
using SmartLock.UI.Features.Settings;
using SmartLock.UI.Features.Settings.DeviceIdService;
using SmartLock.UI.Pages;
using SmartLock.UI.ViewModels;

namespace SmartLock.UI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmartLockUi(this IServiceCollection services, IConfiguration configuration) =>
        services.AddSmartLockServices(configuration)
            .AddSmartLockUIServices()
            .AddSmartLockPages();

    private static IServiceCollection AddSmartLockServices(this IServiceCollection services,
        IConfiguration configuration) =>
        services.AddSmartLockClient(configuration.GetValue<string?>("SmartLock:ApiUrl") ??
                                    throw new InvalidOperationException("SmartLock:ApiUrl is not set."))
            .AddTransient<IDeviceIdService, DeviceIdService>()
            .AddSingleton<IClientSettingsMediator, ClientSettingsMediator>()
            .AddSingleton<IObservable<ClientSettings>>(
                sp => sp.GetRequiredService<IClientSettingsMediator>().Observable);

    private static IServiceCollection AddSmartLockUIServices(this IServiceCollection services) => 
        services.AddSingleton<IMediaPicker, AppMediaPicker>().AddTransient<LockViewModelFactory>().AddTransient<BuildingViewModelFactory>().AddSingleton<AppShell, ShellViewModel>();
    private static IServiceCollection AddSmartLockPages(this IServiceCollection services)
    {
        Routing.RegisterRoute(nameof(BuildingPage), typeof(BuildingPage));
        return services.AddSingleton<DetectionPage, DetectionViewModel>()
            .AddSingleton<BuildingsPage, BuildingsViewModel>()
            .AddTransient<BuildingPage, BuildingViewModel>()
            .AddSingleton<HomePage, HomeViewModel>()
            .AddSingleton<SettingsPage, SettingsViewModel>()
            .AddSingleton<NotificationsStatusView, NotificationsStatusViewModel>();
    }
}