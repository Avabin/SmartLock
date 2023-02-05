using Microsoft.Extensions.DependencyInjection;
using SmartLock.Client.HttpClient;
using SmartLock.Client.Models;
using SmartLock.Client.NotificationHub;

namespace SmartLock.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmartLockClient(this IServiceCollection services, string baseUrl)
    {
        services.AddTransient<IClientSettingsService, ClientSettingsService>();
        services.AddTransient<IDetectionService, DetectionService>();
        services.AddTransient<IBuildingsService, BuildingsService>();
        services.AddSingleton<ObservableNotificationReceiver>();
        services.AddSingleton<INotificationReceiver>(provider => provider.GetRequiredService<ObservableNotificationReceiver>());
        services.AddSingleton<IObservable<Notification<IEvent>>>(provider => provider.GetRequiredService<ObservableNotificationReceiver>());
        services.AddSingleton<INotificationsHubClient>(provider => new NotificationsHubClient(baseUrl, provider.GetServices<INotificationReceiver>()));
        services.AddHttpClient(HttpClientConstants.Name, client => client.BaseAddress = new Uri(baseUrl));
        return services;
    }
}