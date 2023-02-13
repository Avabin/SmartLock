using System.Net;
using System.Net.Sockets;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using OpenTelemetry.Trace;
using Orleans.Configuration;
using SmartLock.Streams.RabbitMQ.Configurators;

namespace SmartLock.Config.Strategies.Silo;

public class MongoSiloStrategy : SiloStrategyBase
{
    public override void ConfigureSilo(ISiloBuilder builder, IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetConnectionString("Mongo");
        var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(mongoConnectionString));
        clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());
        builder.Services.Configure<ClusterOptions>(configuration.GetSection("Orleans"));
        var databaseName = configuration.GetValue<string>("Orleans:ClusterId");
        var siloPort = configuration.GetValue<int?>("Orleans:SiloPort") ?? 11111;
        
        var ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork); 
        builder.ConfigureEndpoints(ipAddress, siloPort, 30000);
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing.AddMongoDBInstrumentation();
                
                tracing.AddSource("SmartLock.Streams.RabbitMQProducer");
                tracing.AddSource("SmartLock.Streams.RabbitMQConsumer");
            });
        builder
            .UseMongoDBClient(_ => clientSettings)
            .UseMongoDBClustering(options =>
            {
                options.CollectionPrefix = "Orleans";
                options.DatabaseName = databaseName;
            })
            .AddMongoDBGrainStorageAsDefault(options =>
            {
                options.CollectionPrefix = "Orleans";
                options.DatabaseName = databaseName;
            })
            .AddMongoDBGrainStorage("PubSubStore", options =>
            {
                options.CollectionPrefix = "OrleansPubSub";
                options.DatabaseName = databaseName;
            })
            .UseMongoDBReminders(options =>
            {
                options.CollectionPrefix = "OrleansReminders";
                options.DatabaseName = databaseName;
            })
            .AddLogStorageBasedLogConsistencyProvider()
            .AddRabbitMQStreams(configuration.GetRequiredSection("RabbitMQ"));
    }
}