using Microsoft.Extensions.Logging;
using SmartLock.Client;
using SmartLock.Client.Models;
using SmartLock.UI.ViewModels;
using BuildingsViewModel = SmartLock.UI.ViewModels.Buildings.BuildingsViewModel;

namespace SmartLock.UI;

public static class MauiProgram
{
    private static IDisposable? _sub = null;
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Services.AddSmartLockClient("http://localhost:5067");
        builder.Services.AddTransient<BuildingsViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<LockViewModelFactory>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        
        return app;
    }
}