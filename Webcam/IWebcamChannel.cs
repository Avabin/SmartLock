using System.Drawing;

namespace Webcam;

public interface IWebcamChannel
{
    Size Resolution { get; }
    uint FrameRate { get; }
    IObservable<Memory<byte>> FrameObservable { get; }
}