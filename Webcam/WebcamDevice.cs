using FlashCap;

namespace Webcam;

public class WebcamDevice : IWebcamDevice
{
    private readonly CaptureDeviceDescriptor _descriptor;
    private static Lazy<CaptureDevices> _captureDevices = new(() => new CaptureDevices());
    private static CaptureDevices CaptureDevices => _captureDevices.Value;
    private static Lazy<IEnumerable<CaptureDeviceDescriptor>> _captureDeviceDescriptors = new(() => CaptureDevices.EnumerateDescriptors());

    private WebcamDevice(CaptureDeviceDescriptor descriptor)
    {
        _descriptor = descriptor;
    }

    private static IEnumerable<CaptureDeviceDescriptor> CaptureDeviceDescriptors => _captureDeviceDescriptors.Value;
    
    public static IEnumerable<IWebcamDevice> GetDevices()
    {
        return CaptureDeviceDescriptors.Select(descriptor => new WebcamDevice(descriptor));
    }

    public IWebcamChannel GetChannel(int channel)
    {
        if (channel < 0 || channel >= _descriptor.Characteristics.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(channel));
        }
        return new WebcamChannel(_descriptor, _descriptor.Characteristics[channel]);
    }
}