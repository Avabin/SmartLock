using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using SmartLock.Client.Models;
using SmartLock.GrainInterfaces;
using Yolov8.Client;

namespace SmartLock.Grains;

public class DetectorGrain : Grain, IDetectorGrain
{
    private readonly IYoloClient _client;
    private readonly ILogger<DetectorGrain> _logger;

    public DetectorGrain(ILogger<DetectorGrain> logger, IYoloClient client)
    {
        _logger = logger;
        _client = client;
    }
    
    public async ValueTask<IReadOnlyList<DetectionResult>> DetectAsync(string imgUrl)
    {
        _logger.LogInformation("Detecting objects in image {ImgUrl}", imgUrl);
        if (!Uri.TryCreate(imgUrl, UriKind.Absolute, out _))
        {
            _logger.LogError("Invalid image url {ImgUrl}", imgUrl);
            throw new InvalidOperationException("Invalid image url");
        }
        _logger.LogDebug("Detecting objects in image {ImgUrl}", imgUrl);
        var results = await _client.DetectAsync(imgUrl);
        _logger.LogDebug("Detected {Count} objects in image {ImgUrl}", results.Count, imgUrl);
        _logger.LogTrace("Detected objects {@Results}", results);
        
        return results.Select(x => DetectionResult.FromYolo(x)).ToImmutableList();
    }

    public async ValueTask<IReadOnlyList<DetectionResult>> DetectAsync(DetectionRequest request)
    {
        _logger.LogInformation("Detecting objects in image {ImageLength}", request.Data.Length);
        using var ms = new MemoryStream(request.Data.ToArray());
        var results = await _client.DetectAsync(ms);
        _logger.LogDebug("Detected {Count} objects in image {ImageLength}", results.Count, request.Data.Length);
        _logger.LogTrace("Detected objects {@Results}", results);
        
        return results.Select(x => DetectionResult.FromYolo(x)).ToImmutableList();
    }
}