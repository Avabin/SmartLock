using RestSharp;

namespace CompreFace.Sdk;

public interface IRestClientFactory
{
    RestClient CreateRecognition();
    RestClient CreateDetection();
    RestClient CreateVerification();
}