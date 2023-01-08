using RestSharp;

namespace CompreFace.Sdk.Recognition;

public interface IRecognitionClient
{
    Task<IReadOnlyList<string>> GetSubjectsAsync();
    Task<ExampleSaved> AddExampleAsync(Func<Stream> image, string subject);
    Task<RecognitionDto> RecognizeAsync(Func<FileStream> image, int limit = 10, float threshold = 0.5f, string facePlugins = "all", bool includeSystemInfo = false);
}

internal class RecognitionClient : IRecognitionClient
{
    private Lazy<RestClient> _restClient;
    private RestClient RestClient => _restClient.Value;
    public RecognitionClient(IRestClientFactory clientFactory)
    {
        _restClient = new Lazy<RestClient>(clientFactory.CreateRecognition);
    }
    public async Task<IReadOnlyList<string>> GetSubjectsAsync()
    {
        var result = await RestClient.GetJsonAsync<SubjectsResult>("/api/v1/recognition/subjects");
        
        return result?.Subjects ?? new List<string>();
    }

    public async Task<ExampleSaved> AddExampleAsync(Func<Stream> image, string subject)
    {
        // upload an example of subject to the server
        // image must be uploaded as multipart/form-data
        // subject must be passed as a query parameter
        // POST /api/v1/recognition/faces?subject=<subject>&det_prob_threshold=<det_prob_threshold>"

        var req = new RestRequest()
        {
            Method = Method.Post,
            Resource = "/api/v1/recognition/faces"
        };
        
        req.AddQueryParameter("subject", subject);
        req.AddFile("file", image, "image.jpg");
        var result = await RestClient.PostAsync<ExampleSaved>(req);
        
        return result!;
    }

    public async Task<RecognitionDto> RecognizeAsync(Func<FileStream> image, int limit = 10, float threshold = 0.5f, string facePlugins = "all", bool includeSystemInfo = false)
    {
        var request = new RestRequest("api/v1/recognition/recognize", Method.Post);
        request.AddFile("file", image, "image.jpg");
        request.AddParameter("limit", limit);
        request.AddParameter("det_prob_threshold", threshold);
        request.AddParameter("face_plugins", facePlugins);
        request.AddParameter("status", includeSystemInfo);
        request.AddHeader("Content-Type", "multipart/form-data");

        var response = await RestClient.ExecuteAsync<RecognitionDto?>(request);
        
        return response.Data ?? RecognitionDto.Empty;
    }
}