using System.Drawing;
using System.Reactive.Disposables;
using FlashCap;

namespace Webcam;

public class WebcamChannel : IWebcamChannel
{
    private readonly CaptureDeviceDescriptor _deviceDescriptor;
    private readonly VideoCharacteristics _characteristics;

    public WebcamChannel(CaptureDeviceDescriptor deviceDescriptor, VideoCharacteristics characteristics)
    {
        _deviceDescriptor = deviceDescriptor;
        _characteristics = characteristics;
        Resolution = new Size(characteristics.Width, characteristics.Height);
        FrameRate = (uint)characteristics.FramesPerSecond;
        
    }
    public Size Resolution { get; }
    public uint FrameRate { get; }
    public PixelFormats PixelFormat => _characteristics.PixelFormat; 
    public IDisposable Subscribe(IObserver<Memory<byte>> observer)
    {
        var device = _deviceDescriptor.OpenAsync(_characteristics, async bufferScope =>
        {
            var image = bufferScope.Buffer.ExtractImage();
            
            observer.OnNext(image);
        });
        Task.Run(async () => await (await device).StartAsync());

        return Disposable.Create(device, task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                task.Result.Dispose();
            }

            task.Dispose();
        });
    }
}