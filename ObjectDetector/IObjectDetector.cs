using System.Drawing;
using SkiaSharp;

namespace Yolov7;

public interface IObjectDetector
{
    Task<(TimeSpan elapsed, List<DetectionResult> predictions)> DetectAsync(SKBitmap bitmap);
}