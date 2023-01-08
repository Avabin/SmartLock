namespace Yolo.Core;

public abstract record ImageData<T>(int Width, int Height, T Data) : IImageData
{
    public abstract void Dispose();
}