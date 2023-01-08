using SkiaSharp;

namespace Yolov7.Yolov7;

public static class RectangleExtensions
{
    public static float Area(this SKRect source)
    {
        return source.Width * source.Height;
    }
}