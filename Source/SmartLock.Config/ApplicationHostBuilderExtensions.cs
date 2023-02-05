using System.Net;
using SmartLock.Config.Strategies;
using SmartLock.Config.Strategies.Client;
using SmartLock.Config.Strategies.Silo;
using SmartLock.Orleans.Core;
using SmartLock.Streams.RabbitMQ.Configurators;

namespace SmartLock.Config;

public static class ApplicationHostBuilderExtensions
{
    public static WebApplicationBuilder UseOrleansStrategy(this WebApplicationBuilder builder, Strategy strategy)
    {
        ISiloStrategy siloStrategy = strategy.Value switch
        {
            Strategy.Redis => new RedisSiloStrategy(),
            Strategy.Local => new LocalSiloStrategy(),
            Strategy.Mongo => new MongoSiloStrategy(),
            Strategy.AzureTable => new AzureTableSiloStrategy(),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };
        
        siloStrategy.Configure(builder);
        
        return builder;
    }
    
    public static HostApplicationBuilder UseOrleansStrategy(this HostApplicationBuilder builder, Strategy strategy)
    {
        ISiloStrategy siloStrategy = strategy.Value switch
        {
            Strategy.Redis => new RedisSiloStrategy(),
            Strategy.Local => new LocalSiloStrategy(),
            Strategy.Mongo => new MongoSiloStrategy(),
            Strategy.AzureTable => new AzureTableSiloStrategy(),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };
        
        siloStrategy.Configure(builder);
        
        return builder;
    }
    
    public static HostApplicationBuilder UseOrleansClientStrategy(this HostApplicationBuilder builder, Strategy strategy)
    {
        IClientStrategy clientStrategy = strategy.Value switch
        {
            Strategy.Redis => new RedisClientStrategy(),
            Strategy.Local => new LocalClientStrategy(),
            Strategy.Mongo => new MongoClientStrategy(),
            Strategy.AzureTable => new AzureCloudClientStrategy(),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };
        
        clientStrategy.Configure(builder);
        
        return builder;
    }
    public static WebApplicationBuilder UseOrleansClientStrategy(this WebApplicationBuilder builder, Strategy strategy)
    {
        IClientStrategy clientStrategy = strategy.Value switch
        {
            Strategy.Redis => new RedisClientStrategy(),
            Strategy.Local => new LocalClientStrategy(),
            Strategy.Mongo => new MongoClientStrategy(),
            Strategy.AzureTable => new AzureCloudClientStrategy(),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };
        
        clientStrategy.Configure(builder);
        
        return builder;
    }

    public static ISiloBuilder UseRabbitMqStreams(this ISiloBuilder builder, IConfiguration configuration, string name = StreamProviderConstants.DefaultStreamProviderName)
    {
        
        var rabbitMqSection = configuration.GetSection("RabbitMQ");
        var rabbitHost = rabbitMqSection.GetValue<string?>("Host") ?? "127.0.0.1";
        var rabbitPort = rabbitMqSection.GetValue<int?>("Port") ?? 5672;
        var rabbitUser = rabbitMqSection.GetValue<string?>("UserName") ?? "guest";
        var rabbitPassword = rabbitMqSection.GetValue<string?>("Password") ?? "guest";
        var rabbitVirtualHost = rabbitMqSection.GetValue<string?>("VirtualHost") ?? "/";
        var rabbitClientName = rabbitMqSection.GetValue<string?>("ClientName") ?? $"SmartLock-{Random.Shared.NextInt64():X}";

        

        builder.AddRabbitMQStreams(name, options =>
        {
            options.StreamSystemConfig.Endpoints.Clear();
            options.StreamSystemConfig.Endpoints.Add(new IPEndPoint(IPAddress.Parse("127.0.0.1"), rabbitPort));
            options.StreamSystemConfig.VirtualHost = rabbitVirtualHost;
            options.StreamSystemConfig.UserName = rabbitUser;
            options.StreamSystemConfig.Password = rabbitPassword;
            options.StreamSystemConfig.ClientProvidedName = rabbitClientName;
        });

        return builder;
    }
}