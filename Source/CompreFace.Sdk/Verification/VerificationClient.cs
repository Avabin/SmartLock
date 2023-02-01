using RestSharp;

namespace CompreFace.Sdk.Verification;

internal class VerificationClient : IVerificationClient
{
    private readonly IRestClientFactory _restClientFactory;
    private Lazy<RestClient> _restClient;
    private RestClient RestClient => _restClient.Value;


    public VerificationClient(IRestClientFactory restClientFactory)
    {
        _restClientFactory = restClientFactory;
        
        _restClient = new Lazy<RestClient>(() => _restClientFactory.CreateVerification());
    }
    public async Task<VerificationDto> VerifyAsync(Func<Stream> sourceImage, Func<Stream> targetImage, int limit = 10, float threshold = 0.5f, string facePlugins = "all",
        bool includeSystemInfo = false)
    {
        var request = new RestRequest("/api/v1/verification/verify", Method.Post);
        request.AddFile("source_image", sourceImage, "sourceImage.jpg");
        request.AddFile("target_image", targetImage, "targetImage.jpg");
        request.AddParameter("limit", limit);
        request.AddParameter("det_prob_threshold", threshold);
        request.AddParameter("face_plugins", facePlugins);
        request.AddParameter("status", includeSystemInfo);

        var response = await RestClient.ExecuteAsync<VerificationDto?>(request);
        
        return response.Data ?? VerificationDto.Empty;
    }
}