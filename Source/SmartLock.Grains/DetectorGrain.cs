using System.Collections.Immutable;
using SmartLock.Client.Models;
using SmartLock.GrainInterfaces;

namespace SmartLock.Grains;

public class DetectorGrain : Grain, IDetectorGrain
{
    public async ValueTask<DetectionResult> DetectAsync(string imgUrl)
    {
        if (!Uri.TryCreate(imgUrl, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException("Invalid image url");
        }
        var builder = ImmutableDictionary.CreateBuilder<string, float>();
        await Task.Delay(100); // simulate request to external service or some work
        // simulate non image response from url when url does not ends with .jpg or .png etc
        if (!imgUrl.EndsWith(".jpg") && !imgUrl.EndsWith(".png") && !imgUrl.EndsWith(".jpeg"))
        {
            throw new ArgumentException("Url does not point to image");
        }
        
        
        builder.Add("person", 0.9f);
        builder.Add("car", 0.8f);
        builder.Add("bicycle", 0.7f);
        
        return new DetectionResult(builder.ToImmutable());
    }
}