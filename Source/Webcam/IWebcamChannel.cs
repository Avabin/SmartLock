using System.Drawing;
using FlashCap;

namespace Webcam;

public interface IWebcamChannel : IObservable<Memory<byte>>
{
    Size Resolution { get; }
    uint FrameRate { get; }
    PixelFormats PixelFormat { get; }
}