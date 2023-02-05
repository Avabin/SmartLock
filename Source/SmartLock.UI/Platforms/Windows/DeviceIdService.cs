using Windows.System.Profile;

namespace SmartLock.UI.Services;

public partial class DeviceIdService
{
    public partial string GetDeviceId()
    {
        if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.System.Profile.SystemIdentification"))
        {
            var systemId = Windows.System.Profile.SystemIdentification.GetSystemIdForPublisher();
            var hardwareId = systemId.Id;
            var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);

            byte[] bytes = new byte[hardwareId.Length];
            dataReader.ReadBytes(bytes);

            return BitConverter.ToString(bytes).Replace("-", "");
        }
        
        return "windows";
    }
}