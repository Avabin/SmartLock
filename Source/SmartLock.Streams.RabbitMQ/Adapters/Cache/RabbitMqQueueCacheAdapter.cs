using Orleans.Streams;
using SmartLock.Streams.RabbitMQ.RabbitMQ;

namespace SmartLock.Streams.RabbitMQ.Adapters.Cache;

internal class RabbitMqQueueCacheAdapter : IQueueAdapterCache
{
    private readonly RabbitMqQueueCacheOptions _rabbitMqQueueCacheOptions;

    public RabbitMqQueueCacheAdapter(RabbitMqQueueCacheOptions rabbitMqQueueCacheOptions)
    {
        _rabbitMqQueueCacheOptions = rabbitMqQueueCacheOptions;
    }

    public IQueueCache CreateQueueCache(QueueId queueId) => new RabbitMqQueueCache(_rabbitMqQueueCacheOptions);
}