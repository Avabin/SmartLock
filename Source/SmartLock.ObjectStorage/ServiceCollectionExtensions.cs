using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using SmartLock.ObjectStorage.Minio;

namespace SmartLock.ObjectStorage;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMinioObjectStorage(this IServiceCollection services, Action<MinioOptions> configure)
    {
        services.Configure(configure);
        AddCoreServices(services);
        return services;
    }

    public static IServiceCollection AddMinioObjectStorage(this IServiceCollection services, IConfigurationSection section)
    {
        services.Configure<MinioOptions>(section);
        AddCoreServices(services);
        return services;
    }

    private static void AddCoreServices(IServiceCollection services)
    {
        services.AddSingleton<IMinioClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MinioOptions>>().Value;

            return new MinioClient().WithEndpoint(options.Endpoint, options.Port)
                .WithCredentials(options.AccessKey, options.SecretKey).WithSSL(options.Secure)
                .Build();
        });
        services.AddSingleton<IObjectStorage, MinioObjectStorage>();
    }
}