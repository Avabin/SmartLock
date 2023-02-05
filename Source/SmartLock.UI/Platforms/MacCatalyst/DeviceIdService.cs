namespace SmartLock.UI.Services;

public partial class DeviceIdService
{
    public partial string GetDeviceId()
    {
        return UIKit.UIDevice.CurrentDevice.IdentifierForVendor.ToString();
    }
}