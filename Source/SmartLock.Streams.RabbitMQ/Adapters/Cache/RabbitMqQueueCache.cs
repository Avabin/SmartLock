using System.Collections.Concurrent;
using Orleans.Runtime;
using Orleans.Streams;
using SmartLock.Streams.RabbitMQ.RabbitMQ;

namespace SmartLock.Streams.RabbitMQ.Adapters.Cache;

internal class RabbitMqQueueCache : IQueueCache
{
    private static int _countProcessingMessages;
    private readonly List<RabbitMqBatchContainer> _messagesToPurge = new();

    private readonly ConcurrentDictionary<StreamId, Lazy<ConcurrentQueue<RabbitMqBatchContainer>>>
        _processingMessages = new();

    private readonly RabbitMqQueueCacheOptions _cacheOptions;

    public RabbitMqQueueCache(RabbitMqQueueCacheOptions cacheOptions)
    {
        _cacheOptions = cacheOptions;
    }


    public int GetMaxAddCount() => _cacheOptions.CacheSize;

    public void AddToCache(IList<IBatchContainer> messages)
    {
        Interlocked.Add(ref _countProcessingMessages, messages.Count);
        foreach (var batchContainers in messages.GroupBy(m => m.StreamId))
        {
            _processingMessages.AddOrUpdate(batchContainers.Key,
                _ => new Lazy<ConcurrentQueue<RabbitMqBatchContainer>>(
                    () => new ConcurrentQueue<RabbitMqBatchContainer>(batchContainers.Cast<RabbitMqBatchContainer>()),
                    LazyThreadSafetyMode.ExecutionAndPublication),
                (_, lazyQueue) =>
                    new Lazy<ConcurrentQueue<RabbitMqBatchContainer>>(() =>
                    {
                        foreach (var rabbitMqBatchContainer in batchContainers.Cast<RabbitMqBatchContainer>())
                        {
                            lazyQueue.Value.Enqueue(rabbitMqBatchContainer);
                        }

                        return lazyQueue.Value;
                    }, LazyThreadSafetyMode.ExecutionAndPublication));
        }
    }

    public bool TryPurgeFromCache(out IList<IBatchContainer> purgedItems)
    {
        if (_messagesToPurge.Count < 1)
        {
            purgedItems = null;
            return false;
        }

        purgedItems = _messagesToPurge.Cast<IBatchContainer>().ToList();
        _messagesToPurge.Clear();

        return purgedItems.Count > 0;
    }

    public IQueueCacheCursor GetCacheCursor(StreamId streamId, StreamSequenceToken token)
    {
        var cursor = new RabbitMqQueueCacheCursor(streamId, token, _processingMessages, messageRead =>
        {
            if (messageRead is not null)
                _messagesToPurge.Add(messageRead);

            Interlocked.Decrement(ref _countProcessingMessages);
        }, () => _processingMessages);

        return cursor;
    }

    public bool IsUnderPressure() => _countProcessingMessages >= GetMaxAddCount();

    
}