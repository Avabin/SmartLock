using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

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
        client.UseSerializer<JsonNetSerializer>();
        return client;
    }
    
    public RestClient CreateDetection()
    {
        var client = new RestClient(Options.BaseUrl);
        client.AddDefaultHeader("X-Api-Key", Options.DetectionApiKey.ToString("D"));
        client.UseSerializer<JsonNetSerializer>();
        return client;
    }
    
    public RestClient CreateVerification()
    {
        var client = new RestClient(Options.BaseUrl);
        client.AddDefaultHeader("X-Api-Key", Options.VerificationApiKey.ToString("D"));
        client.UseSerializer<JsonNetSerializer>();
        return client;
    }
}