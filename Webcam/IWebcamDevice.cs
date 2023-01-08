using System.Drawing;

namespace Webcam;

public interface IWebcamDevice
{
    Size Resolution { get; }
    uint FrameRate { get;  }
    IObservable<Memory<byte>> FrameObservable { get; }
    
    void Start();
    void Stop();
}