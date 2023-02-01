using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Orleans.Serialization;
using RabbitMQ.Stream.Client;
using SmartLock.Streams.RabbitMQ.Adapters;
using SmartLock.Streams.RabbitMQ.RabbitMQ;

namespace SmartLock.Streams.RabbitMQ.Configurators;

public class RabbitMQClientConfigurator : ClusterClientPersistentStreamConfigurator
{
    public RabbitMQClientConfigurator(string providerName, IClientBuilder clientBuilder) : base(providerName, clientBuilder, RabbitMQAdapterFactory.Create)
    {
        ConfigureDelegate(services =>
        {
            services.AddSingleton(sp => new RabbitMQQueueProvider(sp.GetService<RabbitMQStreamSystemProvider>(),
                providerName, sp.GetOptionsByName<RabbitMQClientOptions>(providerName)));
            services.AddSingleton(sp => new RabbitMQAdapterReceiverFactory(sp.GetService<ILoggerFactory>(),
                    sp.GetService<Serializer>(), sp.GetOptionsByName<RabbitMQClientOptions>(providerName)))
                .AddSingleton(sp =>
                    new RabbitMQStreamSystemProvider(sp.GetOptionsByName<RabbitMQClientOptions>(providerName),
                        sp.GetService<ILogger<RabbitMQStreamSystemProvider>>(), sp.GetService<ILogger<StreamSystem>>()))
                .ConfigureNamedOptionForLogging<RabbitMQClientOptions>(providerName)
                .ConfigureNamedOptionForLogging<HashRingStreamQueueMapperOptions>(providerName);
        });
    }
    
    public RabbitMQClientConfigurator ConfigureOffsetUpdateInterval(TimeSpan interval)
    {
        this.Configure<RabbitMQClientOptions>(opt => opt.Configure(e => e.IntervalToUpdateOffset = interval));
        return this;
    }

    public RabbitMQClientConfigurator ConfigureRabbitMQ(Action<OptionsBuilder<RabbitMQClientOptions>> configureOptions)
    {
        this.Configure(configureOptions);
        return this;
    }

    public RabbitMQClientConfigurator ConfigureCache(int cacheSize = RabbitMqQueueCacheOptions.DEFAULT_CACHE_SIZE)
    {
        this.Configure<RabbitMqQueueCacheOptions>(ob => ob.Configure(options => options.CacheSize = cacheSize));
        return this;
    }

    public RabbitMQClientConfigurator ConfigurePartitioning(
        int numOfparitions = HashRingStreamQueueMapperOptions.DEFAULT_NUM_QUEUES)
    {
        this.Configure<HashRingStreamQueueMapperOptions>(ob =>
            ob.Configure(options => options.TotalQueueCount = numOfparitions));
        return this;
    }
}