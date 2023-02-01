using System.Globalization;
using CompreFace.Sdk.Detection.Models;
using FlashCap;
using RestSharp;

namespace CompreFace.Sdk.Detection;

public interface IDetectionClient
{
    Task<DetectionDto> DetectAsync(Func<Stream> stream,PixelFormats pixelFormat, int limit = 10, float threshold = 0.5f, string facePlugins = "all", bool includeSystemInfo = false);
}

internal class DetectionClient : IDetectionClient
{
    private Lazy<RestClient> _restClient;
    private RestClient RestClient => _restClient.Value;

    public DetectionClient(IRestClientFactory clientFactory)
    {
        _restClient = new(clientFactory.CreateDetection);
    }

    public async Task<DetectionDto> DetectAsync(Func<Stream> stream, PixelFormats pixelFormat, int limit = 10,  float threshold = 0.5f, string facePlugins = "all",
        bool includeSystemInfo = false)
    {
        var request = new RestRequest("/api/v1/detection/detect", Method.Post);
        request.AddQueryParameter("limit", limit);
        request.AddQueryParameter("det_prob_threshold", threshold.ToString(CultureInfo.InvariantCulture));
        request.AddQueryParameter("face_plugins", facePlugins);
        request.AddQueryParameter("status", includeSystemInfo);

        switch (pixelFormat)
        {
            case PixelFormats.JPEG or PixelFormats.RGB24:
                request.AddFile("file", stream, "image.jpg");
                break;
            case PixelFormats.PNG:
                request.AddFile("file", stream, "image.png");
                break;
            default:
                throw new ArgumentException($"Unsupported pixel format {pixelFormat:G}");
        }
        request.AddHeader("Content-Type", "multipart/form-data");
        var response = await RestClient.ExecutePostAsync<DetectionDto>(request);
        if (response.Content.Contains("No face is found"))
        {
            return DetectionDto.Empty;
        }

        return response.Data ?? DetectionDto.Empty;
    }
}