using OpenTelemetry.Trace;
using SmartLock.Orleans.Core;
using SmartLock.Streams.RabbitMQ.Configurators;

namespace SmartLock.Config.Strategies.Client;

public class RedisClientStrategy : ClientStrategyBase
{
    public override void ConfigureClient(IClientBuilder builder, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost";
        builder.Services.AddOpenTelemetry().WithTracing(x => x.AddRedisInstrumentation().AddSource("RabbitMQConsumer").AddSource("RabbitMQProducer"));
        builder.UseRedisClustering(options =>
        {
            options.ConnectionString = redisConnectionString;
        }).AddRabbitMQStreams(StreamProviderConstants.DefaultStreamProviderName, configuration.GetRequiredSection("RabbitMQ"));
    }
}