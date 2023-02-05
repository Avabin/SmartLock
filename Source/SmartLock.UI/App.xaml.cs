namespace SmartLock.UI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = MauiProgram.Services.GetRequiredService<AppShell>();
    }
}