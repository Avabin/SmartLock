using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Serialization;
using Orleans.Streams;
using SmartLock.Streams.RabbitMQ.Adapters.Cache;
using SmartLock.Streams.RabbitMQ.RabbitMQ;

namespace SmartLock.Streams.RabbitMQ.Adapters;

internal class RabbitMQAdapterFactory : IQueueAdapterFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly string _providerName;
    private readonly RabbitMQAdapterReceiverFactory _receiverFactory;
    private readonly Serializer _serializer;
    private readonly HashRingBasedStreamQueueMapper _streamQueueMapper;
    private readonly RabbitMQStreamSystemProvider _streamSystemProvider;
    private readonly RabbitMQQueueProvider _rabbitMqQueueProvider;
    private readonly RabbitMqQueueCacheAdapter _queueAdapterCache;

    public RabbitMQAdapterFactory(ILoggerFactory loggerFactory, string providerName,
        RabbitMqQueueCacheOptions rabbitMqQueueCacheOptions,
        RabbitMQClientOptions rabbitMqClientOptions, RabbitMQAdapterReceiverFactory receiverFactory,
        Serializer serializer, RabbitMQStreamSystemProvider streamSystemProvider, RabbitMQQueueProvider rabbitMqQueueProvider, HashRingStreamQueueMapperOptions hashRingStreamQueueMapperOptions)
    {
        _loggerFactory = loggerFactory;
        _providerName = providerName;
        _receiverFactory = receiverFactory;
        _serializer = serializer;
        _streamQueueMapper = rabbitMqClientOptions.QueueNames is null or { Count: 0 }
            ? new HashRingBasedStreamQueueMapper(hashRingStreamQueueMapperOptions, providerName)
            : new HashRingBasedPartitionedStreamQueueMapper(rabbitMqClientOptions.QueueNames, providerName);
        _streamSystemProvider = streamSystemProvider;
        _rabbitMqQueueProvider = rabbitMqQueueProvider;
        _queueAdapterCache = new RabbitMqQueueCacheAdapter(rabbitMqQueueCacheOptions);
    }

    public Task<IQueueAdapter> CreateAdapter()
        => Task.FromResult<IQueueAdapter>(
            new RabbitMQAdapter(_streamQueueMapper, _rabbitMqQueueProvider, _streamSystemProvider, _loggerFactory,
                _receiverFactory, _serializer, _providerName));

    public IQueueAdapterCache GetQueueAdapterCache() => _queueAdapterCache;
    //public IQueueAdapterCache GetQueueAdapterCache() => new SimpleQueueAdapterCache(new SimpleQueueCacheOptions(), "RabbitMQ", _loggerFactory);

    public IStreamQueueMapper GetStreamQueueMapper() => _streamQueueMapper;

    public Task<IStreamFailureHandler> GetDeliveryFailureHandler(QueueId _) =>
        Task.FromResult<IStreamFailureHandler>(new NoOpStreamDeliveryFailureHandler());

    public static RabbitMQAdapterFactory Create(IServiceProvider serviceProvider, string providerName)
    {
        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
        var rabbitMqClientOptions = serviceProvider.GetOptionsByName<RabbitMQClientOptions>(providerName);
        var rabbitMqQueueCacheOptions = serviceProvider.GetOptionsByName<RabbitMqQueueCacheOptions>(providerName);
        var receiverFactory = serviceProvider.GetService<RabbitMQAdapterReceiverFactory>();
        var serializer = serviceProvider.GetService<Serializer>();
        var streamProvider = serviceProvider.GetService<RabbitMQStreamSystemProvider>();
        var rabbitMqQueueProvider = serviceProvider.GetService<RabbitMQQueueProvider>();
        var hashRingStreamQueueMapperOptions = serviceProvider.GetOptionsByName<HashRingStreamQueueMapperOptions>(providerName);

        return new RabbitMQAdapterFactory(loggerFactory, providerName,
            rabbitMqQueueCacheOptions, rabbitMqClientOptions, receiverFactory, serializer, streamProvider, rabbitMqQueueProvider, hashRingStreamQueueMapperOptions);
    }
}