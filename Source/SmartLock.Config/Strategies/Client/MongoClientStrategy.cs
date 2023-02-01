using Orleans.Configuration;
using SmartLock.Orleans.Core;
using SmartLock.Streams.RabbitMQ.Configurators;

namespace SmartLock.Config.Strategies.Client;

public class MongoClientStrategy : ClientStrategyBase
{
    public override void ConfigureClient(IClientBuilder builder, IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetConnectionString("Mongo") ?? "mongodb://localhost:27017";
        builder.Services.Configure<ClusterOptions>(configuration.GetSection("Orleans"));
        var databaseName = configuration.GetValue<string>("Orleans:ClusterId");
        builder.UseMongoDBClient(mongoConnectionString)
            .UseMongoDBClustering(options =>
            {
                options.CollectionPrefix = "Orleans";
                options.DatabaseName = databaseName;
            })
            .AddRabbitMQStreams(StreamProviderConstants.DefaultStreamProviderName, configuration.GetRequiredSection("RabbitMQ"));
    }
}