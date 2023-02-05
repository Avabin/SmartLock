using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Orleans.TestingHost;
using SmartLock.Orleans.Core;
using SmartLock.Streams.RabbitMQ.Configurators;
using Yolov8.Client;

namespace Tests.Shared.SiloConfigurators;

public class RedisTestSiloBuilderConfigurator : ISiloConfigurator
{
    public static string DbName => $"dev{Random.Shared.Next()}";

    private static Dictionary<string, string?> Configs { get; } = new()
    {
        { "ConnectionStrings:Redis", "localhost:6379" },
        { "RabbitMQ:Host", "127.0.0.1" },
        { "RabbitMQ:Port", "5552" },
        { "RabbitMQ:UserName", "guest" },
        { "RabbitMQ:Password", "guest" },
        { "RabbitMQ:VirtualHost", "/" }
    };

    public void Configure(ISiloBuilder siloBuilder)
    {
        var configuration = new ConfigurationBuilder().AddEnvironmentVariables("DOTNET_").Build();
        var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";

        var yoloBaseUrl = Environment.GetEnvironmentVariable("YOLO_BASE_URL") ?? "http://localhost:50051";
        siloBuilder.Services.AddYoloClient(yoloBaseUrl);
        var rabbitSection = configuration.GetSection("RabbitMQ").Value is null ? new ConfigurationBuilder().AddInMemoryCollection(Configs).Build().GetSection("RabbitMQ")
                : configuration.GetSection("RabbitMQ");
        siloBuilder
            .AddRedisGrainStorageAsDefault(options => { options.ConnectionString = redisConnectionString; })
            .AddRedisGrainStorage("PubSubStore", options => { options.ConnectionString = redisConnectionString; })
            .UseRedisClustering(options => { options.ConnectionString = redisConnectionString; })
            .AddRabbitMQStreams(rabbitSection);

        siloBuilder.AddLogStorageBasedLogConsistencyProviderAsDefault();
    }
}