namespace SmartLock.UI.Features.Settings;

public partial class SettingsView : ContentView
{
    public SettingsView()
    {
        BindingContext = MauiProgram.Services.GetRequiredService<SettingsViewModel>(); 
        InitializeComponent();
    }
}