using Microsoft.Extensions.DependencyInjection;
using Yolo.Core;

namespace ObjectDetector.Yolo.Onnx;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddYoloOnnxObjectDetector(this IServiceCollection services, bool useCuda)
    {
        services.AddTransient<IYoloModelProvider, YoloModelProvider>();
        services.AddSingleton<IObjectDetector, YoloObjectDetector>().Configure<ObjectDetectorOptions>(x => x.UseCuda = useCuda);
        return services;
    }
}