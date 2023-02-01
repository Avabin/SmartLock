using System.Net;
using System.Net.Sockets;
using Orleans.Configuration;
using SmartLock.Streams.RabbitMQ.Configurators;

namespace SmartLock.Config.Strategies.Silo;

public class MongoSiloStrategy : SiloStrategyBase
{
    public override void ConfigureSilo(ISiloBuilder builder, IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetConnectionString("Mongo");
        builder.Services.Configure<ClusterOptions>(configuration.GetSection("Orleans"));
        var databaseName = configuration.GetValue<string>("Orleans:ClusterId");
        var siloPort = configuration.GetValue<int?>("Orleans:SiloPort") ?? 11111;
        
        var ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork); 
        builder.ConfigureEndpoints(ipAddress, siloPort, 30000);
        builder
            .UseMongoDBClient(mongoConnectionString ?? throw new InvalidOperationException("Mongo connection string is null"))
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