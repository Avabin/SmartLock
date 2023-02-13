using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using OpenTelemetry.Trace;
using Orleans.Configuration;
using SmartLock.Orleans.Core;
using SmartLock.Streams.RabbitMQ.Configurators;

namespace SmartLock.Config.Strategies.Client;

public class MongoClientStrategy : ClientStrategyBase
{
    public override void ConfigureClient(IClientBuilder builder, IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetConnectionString("Mongo") ?? "mongodb://localhost:27017";
        var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(mongoConnectionString));
        clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());
        builder.Services.Configure<ClusterOptions>(configuration.GetSection("Orleans"));
        var databaseName = configuration.GetValue<string>("Orleans:ClusterId");
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing.AddMongoDBInstrumentation();
                
                
                tracing.AddSource("SmartLock.Streams.RabbitMQProducer");
                tracing.AddSource("SmartLock.Streams.RabbitMQConsumer");
            });
        builder.UseMongoDBClient(_ => clientSettings)
            .UseMongoDBClustering(options =>
            {
                options.CollectionPrefix = "Orleans";
                options.DatabaseName = databaseName;
            })
            .AddRabbitMQStreams(StreamProviderConstants.DefaultStreamProviderName, configuration.GetRequiredSection("RabbitMQ"));
    }
}