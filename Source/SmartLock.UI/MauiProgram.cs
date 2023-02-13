using System.Reflection;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using SmartLock.Client.NotificationHub;

namespace SmartLock.UI;

public static class MauiProgram
{
public static IServiceProvider Services { get; private set; } = null!;
    public static MauiApp CreateMauiApp(string[]? args = null)
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseSkiaSharp(true)
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("materialdesignicons-webfont.ttf", "MaterialIconsRegular");
            });
        if (args is not null) builder.Configuration.AddCommandLine(args);
        builder.Configuration.AddEnvironmentVariables("SM_");
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("SmartLock.UI.appsettings.json");
        if (stream is not null) builder.Configuration.AddJsonStream(stream);
        
        builder.Services.AddSmartLockUi(builder.Configuration);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        Services = app.Services;

        Task.Run(async () =>
        {
            var hubService =(NotificationsHubClient) app.Services.GetRequiredService<INotificationsHubClient>();
            await hubService.StartAsync();
        }).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                var logger = app.Services.GetRequiredService<ILogger<NotificationsHubClient>>();
                logger.LogError(task.Exception, "Failed to start notification hub client");
                WeakReferenceMessenger.Default.Send(new NotificationHubClientFailedToStartMessage());
            }
        });

        return app;
    }
}