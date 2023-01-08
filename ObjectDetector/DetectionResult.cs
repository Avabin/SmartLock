using System.Drawing;
using SkiaSharp;

namespace Yolov7;

public record DetectionResult(string Label, float Confidence, SKRect BoundingBox);