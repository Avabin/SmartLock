using Microsoft.Extensions.DependencyInjection;

namespace Yolov8.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddYoloClient(this IServiceCollection services, string baseUrl, string protocol = "grpc")
    {
        services.Configure<YoloClientOptions>(x =>
        {
            x.Protocol = protocol;
            x.BaseUrl = baseUrl;
        });
        switch (protocol)
        {
            case "grpc":
                services.AddYoloGrpcClient();
                break;
            case "http" or "https":
                services.AddHttpClient(YoloClientConstants.Name, client => client.BaseAddress = new Uri(baseUrl));
                services.AddTransient<IYoloClient, YoloClient>();
                break;
        }

        return services;
    }
    
    private static IServiceCollection AddYoloGrpcClient(this IServiceCollection services)
    {
        services.AddSingleton<IYoloClient, GrpcYoloClient>();

        return services;
    }
}