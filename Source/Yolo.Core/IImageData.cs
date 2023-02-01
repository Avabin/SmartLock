namespace Yolo.Core;

public interface IImageData : IDisposable
{
    int Width { get; }
    int Height { get; }
}