using Microsoft.Extensions.Options;
using RestSharp;

namespace CompreFace.Sdk;

internal class RestClientFactory : IRestClientFactory
{
    private readonly IOptions<CompreFaceSdkOptions> _options;
    private CompreFaceSdkOptions Options => _options.Value;

    public RestClientFactory(IOptions<CompreFaceSdkOptions> options)
    {
        _options = options;
    }
    public RestClient CreateRecognition()
    {
        var client = new RestClient(Options.BaseUrl);
        client.AddDefaultHeader("X-Api-Key", Options.RecognitionApiKey.ToString("D"));
        return client;
    }
    
    public RestClient CreateDetection()
    {
        var client = new RestClient(Options.BaseUrl);
        client.AddDefaultHeader("X-Api-Key", Options.DetectionApiKey.ToString("D"));
        return client;
    }
    
    public RestClient CreateVerification()
    {
        var client = new RestClient(Options.BaseUrl);
        client.AddDefaultHeader("X-Api-Key", Options.VerificationApiKey.ToString("D"));
        return client;
    }
}