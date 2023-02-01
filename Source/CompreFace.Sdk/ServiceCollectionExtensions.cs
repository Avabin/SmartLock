using CompreFace.Sdk.Detection;
using CompreFace.Sdk.Recognition;
using CompreFace.Sdk.Verification;
using Microsoft.Extensions.DependencyInjection;

namespace CompreFace.Sdk;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCompreFaceSdk(this IServiceCollection services, Action<CompreFaceSdkOptions> configure)
    {
        services.Configure(configure);
        services.AddTransient<IRestClientFactory, RestClientFactory>();
        
        services.AddTransient<IDetectionClient, DetectionClient>();
        services.AddTransient<IVerificationClient, VerificationClient>();
        services.AddTransient<IRecognitionClient, RecognitionClient>();
        
        services.AddTransient<ICompreFaceClient, CompreFaceClient>();
        return services;
    }
}