using SmartLock.Client.Models;

namespace SmartLock.GrainInterfaces;

public interface IDetectorGrain : IGrainWithGuidKey
{
    /// <summary>
    /// Detect objects in the image
    /// </summary>
    /// <param name="imgUrl">The image url</param>
    /// <returns>The detected objects</returns>
    ValueTask<DetectionResult> DetectAsync(string imgUrl);
}