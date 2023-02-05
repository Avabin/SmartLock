using SmartLock.Client.Models;
using Yolov8.Client;

namespace SmartLock.Client.HttpClient;

public interface IDetectionService
{
    Task<IReadOnlyList<DetectedObjectModel>> DetectAsync(Stream image);
    Task<IReadOnlyList<DetectedObjectModel>> DetectAsync(string imageUrl);
}

internal class DetectionService : IDetectionService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DetectionService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task<IReadOnlyList<DetectedObjectModel>> DetectAsync(Stream image)
    {
        var client = _httpClientFactory.CreateClient(HttpClientConstants.Name);
        
        return await client.DetectAsync(image);
    }

    public Task<IReadOnlyList<DetectedObjectModel>> DetectAsync(string imageUrl)
    {
        var client = _httpClientFactory.CreateClient(HttpClientConstants.Name);
        
        return client.DetectAsync(imageUrl);
    }
}