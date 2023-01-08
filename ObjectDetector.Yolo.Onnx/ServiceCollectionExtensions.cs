using Microsoft.Extensions.DependencyInjection;

namespace Yolov7;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddYoloObjectDetector(this IServiceCollection services, bool useCuda)
    {
        services.AddTransient<IYoloModelProvider, YoloModelProvider>();
        services.AddSingleton<IObjectDetector, YoloObjectDetector>().Configure<ObjectDetectorOptions>(x => x.UseCuda = useCuda);
        return services;
    }
}