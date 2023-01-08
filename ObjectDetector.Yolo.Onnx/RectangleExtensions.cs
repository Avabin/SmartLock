using SkiaSharp;

namespace ObjectDetector.Yolo.Onnx;

public static class RectangleExtensions
{
    public static float Area(this SKRect source)
    {
        return source.Width * source.Height;
    }
}