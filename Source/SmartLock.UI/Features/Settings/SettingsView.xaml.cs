using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLock.UI.Features.Settings;

public partial class SettingsView : ContentView
{
    public SettingsView()
    {
        BindingContext = MauiProgram.Services.GetRequiredService<SettingsViewModel>(); 
        InitializeComponent();
    }
}