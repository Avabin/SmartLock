using SkiaSharp;

namespace ObjectDetector;

public record DetectionResult(string Label, float Confidence, SKRect BoundingBox);