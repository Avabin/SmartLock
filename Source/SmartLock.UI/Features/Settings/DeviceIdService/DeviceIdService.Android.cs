namespace SmartLock.UI.Features.Settings.DeviceIdService;

public partial class DeviceIdService
{
    public partial string GetDeviceId()
    {
        var id = Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
        
        return id;
    }
}