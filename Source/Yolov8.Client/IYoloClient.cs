namespace Yolov8.Client;

public interface IYoloClient
{
    Task<IReadOnlyList<YoloDetectionModel>> DetectAsync(Stream image, float confidence = 0.5f, float intersectionOverUnion = 0.5f);
    Task<IReadOnlyList<YoloDetectionModel>> DetectAsync(string imageUrl, float confidence = 0.5f, float intersectionOverUnion = 0.5f);
}

internal class YoloClient : IYoloClient
{
    private HttpClient _client;
    public YoloClient(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient(YoloClientConstants.Name);
    }
    public async Task<IReadOnlyList<YoloDetectionModel>> DetectAsync(Stream image, float confidence = 0.5f, float intersectionOverUnion = 0.5f) => 
        await _client.DetectAsync(image);

    public Task<IReadOnlyList<YoloDetectionModel>> DetectAsync(string imageUrl, float confidence = 0.5f, float intersectionOverUnion = 0.5f) => 
        _client.DetectAsync(imageUrl);
}