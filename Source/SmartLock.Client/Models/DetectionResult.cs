using System.Collections.Immutable;
using Yolov8.Client;

namespace SmartLock.Client.Models;

/// <summary>
/// Object or person detection result
/// </summary>
/// <param name="Detections">Dictionary of detected objects or persons</param>
[Immutable, GenerateSerializer]
public record DetectionResult([property: Id(0)][Immutable] ImmutableList<DetectedObjectModel> Detections)
{
    // empty
    public static DetectionResult Empty => new(ImmutableList<DetectedObjectModel>.Empty);

    public static DetectionResult FromYolo(params YoloDetectionModel[] detectionsModels) =>
        new(detectionsModels.Select(x => new DetectedObjectModel(x.ClassName, x.Confidence, x.X1, x.Y1, x.X2, x.Y2)).ToImmutableList());
};