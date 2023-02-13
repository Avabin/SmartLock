namespace SmartLock.UI.Features.Settings.DeviceIdService;

public partial class DeviceIdService
{
    public partial string GetDeviceId()
    {
        return UIKit.UIDevice.CurrentDevice.IdentifierForVendor.ToString();
    }
}