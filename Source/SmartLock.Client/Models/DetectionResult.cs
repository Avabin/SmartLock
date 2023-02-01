using System.Collections.Immutable;

namespace SmartLock.Client.Models;

/// <summary>
/// Object or person detection result
/// </summary>
/// <param name="Detections">Dictionary of detected objects or persons</param>
[Immutable, GenerateSerializer]
public record DetectionResult([property: Id(0)][Immutable] ImmutableDictionary<string, float> Detections)
{
    // empty
    public static DetectionResult Empty => new(ImmutableDictionary<string, float>.Empty);
};