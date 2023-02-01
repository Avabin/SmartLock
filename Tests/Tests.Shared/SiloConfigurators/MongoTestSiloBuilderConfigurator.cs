using System.Net;
using Microsoft.Extensions.Configuration;
using Orleans.TestingHost;
using SmartLock.Orleans.Core;
using SmartLock.Streams.RabbitMQ.Configurators;

namespace Tests.Shared.SiloConfigurators;

public class MongoTestSiloBuilderConfigurator : ISiloConfigurator
{
    public static string DbName => $"dev{Random.Shared.Next()}";
    private static Dictionary<string, string?> Configs { get; } = new()
    {
        {"ConnectionStrings:Mongo", "mongodb://localhost:27017"},
        {"RabbitMQ:Host", "127.0.0.1"},
        {"RabbitMQ:Port", "5552"},
        {"RabbitMQ:UserName", "guest"},
        {"RabbitMQ:Password", "guest"},
        {"RabbitMQ:VirtualHost", "/"}
    };
    public void Configure(ISiloBuilder siloBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(Configs)
            .Build();
        var mongoConnectionString = configuration.GetConnectionString("Mongo");
        var dbName = MongoTestSiloBuilderConfigurator.DbName;
        siloBuilder.UseMongoDBClient(mongoConnectionString)
            .AddMongoDBGrainStorageAsDefault(options => { options.DatabaseName = dbName; })
            .AddMongoDBGrainStorage("PubSubStore", options => { options.DatabaseName = dbName; })
            .UseMongoDBClustering(options => { options.DatabaseName = dbName; })
            .UseMongoDBReminders(options => { options.DatabaseName = dbName; })
            .AddLogStorageBasedLogConsistencyProviderAsDefault()
            .AddRabbitMQStreams(configuration.GetRequiredSection("RabbitMQ"));
    }
}