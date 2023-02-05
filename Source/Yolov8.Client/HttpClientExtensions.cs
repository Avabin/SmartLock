using System.Net.Http.Json;
using System.Text;

namespace Yolov8.Client;

internal static class HttpClientExtensions
{
    internal static async Task<IReadOnlyList<YoloDetectionModel>> DetectAsync(this HttpClient client, string imageUrl)
    {
        var response = await client.PostAsync("predict_url", new StringContent(imageUrl, Encoding.UTF8, "text/plain")).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<IReadOnlyList<YoloDetectionModel>>().ConfigureAwait(false) ?? Array.Empty<YoloDetectionModel>();
    }
    internal static async Task<IReadOnlyList<YoloDetectionModel>> DetectAsync(this HttpClient client, Stream image)
    {
        // send image as form file in field `image`
        var form = new MultipartFormDataContent();
        form.Add(new StreamContent(image), "image", "image.jpg");
        var response = await client.PostAsync("predict", form).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<IReadOnlyList<YoloDetectionModel>>().ConfigureAwait(false) ?? Array.Empty<YoloDetectionModel>();
    }
}