using System.Collections.Concurrent;
using Orleans.Runtime;
using Orleans.Streams;

namespace SmartLock.Streams.RabbitMQ.Adapters.Cache;

internal class RabbitMqQueueCacheCursor : IQueueCacheCursor
{
    private readonly StreamId _streamId;
    private StreamSequenceToken _hadnshakeToken;
    private ConcurrentDictionary<StreamId, Lazy<ConcurrentQueue<RabbitMqBatchContainer>>> _processingMessages;
    private readonly Action<RabbitMqBatchContainer> _onMessageRead;
    private readonly Func<ConcurrentDictionary<StreamId, Lazy<ConcurrentQueue<RabbitMqBatchContainer>>>> _onPreRefresh;
    private RabbitMqBatchContainer _current;

    public RabbitMqQueueCacheCursor(StreamId streamId, StreamSequenceToken hadnshakeToken,
        ConcurrentDictionary<StreamId, Lazy<ConcurrentQueue<RabbitMqBatchContainer>>> processingMessages,
        Action<RabbitMqBatchContainer> onMessageRead,
        Func<ConcurrentDictionary<StreamId, Lazy<ConcurrentQueue<RabbitMqBatchContainer>>>> onPreRefresh)
    {
        _streamId = streamId;
        _hadnshakeToken = hadnshakeToken;
        _processingMessages = processingMessages;
        _onMessageRead = onMessageRead;
        _onPreRefresh = onPreRefresh;
        Initialize();
    }

    private void Initialize(bool isRefreshing = false)
    {
        if (((_hadnshakeToken?.SequenceNumber ?? 0) != 0) && _processingMessages.TryGetValue(_streamId, out var queue))
        {

            //Check if next item in the stream queue is the one we are looking for
            //If it is, the cursor is already where it should be
            if (queue.Value.TryPeek(out var nextItem) && nextItem.SequenceToken.SequenceNumber >= _hadnshakeToken.SequenceNumber)
            {
                if (isRefreshing && nextItem.SequenceToken.SequenceNumber == _hadnshakeToken.SequenceNumber)
                {
                    MoveNext();
                }
                return;
            }

            //Try to find the last consumed message
            nextItem = MoveToTokenTargetMessage(queue, isRefreshing);


            //If at this point we couldn't find the last consumed message,
            //it means that the pulling agent is still reading messages and this message we are looking for is still coming in the future,
            //so we CAN NOT deliver this cursor to the consumer because it would deliver messages it has already read
            if (nextItem is null || nextItem.SequenceToken.SequenceNumber != _hadnshakeToken.SequenceNumber && _current.SequenceToken.SequenceNumber < _hadnshakeToken.SequenceNumber)
            {
                //make last message ready to purge
                MoveNext();

                //Create an empty message queue to make consumer inactive.
                //It will be ready again once the message it wants to consume arrives
                _processingMessages =
                    new ConcurrentDictionary<StreamId, Lazy<ConcurrentQueue<RabbitMqBatchContainer>>>();
            }
        }
    }

    private RabbitMqBatchContainer MoveToTokenTargetMessage(Lazy<ConcurrentQueue<RabbitMqBatchContainer>> queue, bool moveToProcessedToken = false)
    {
        RabbitMqBatchContainer nextItem;
        var hasNext = false;
        do
        {
            hasNext = MoveNext();
            queue.Value.TryPeek(out nextItem);
        } while (hasNext && nextItem is not null && nextItem.SequenceToken.SequenceNumber != _hadnshakeToken.SequenceNumber);

        if (nextItem is not null && nextItem.SequenceToken.Equals(_hadnshakeToken) && moveToProcessedToken)
        {
            MoveNext();
            queue.Value.TryPeek(out nextItem);
        }

        return nextItem;
    }

    public void Dispose()
    {
        //nothing to do here
    }

    public IBatchContainer GetCurrent(out Exception exception)
    {
        exception = null;
        return _current;
    }

    public bool MoveNext()
    {
        //The current message has failed to process, we can't move the cursor
        if (_current is { DeliveryFailed: true }) { return false; }

        RabbitMqBatchContainer message = null;
        var streamExisted = _processingMessages.TryGetValue(_streamId, out var lazyQueue);
        switch (streamExisted)
        {
            case true when _current is null:
                lazyQueue.Value.TryDequeue(out message);

                _current = message;
                return _current is not null;
            case true:
            {
                _onMessageRead.Invoke(_current);
                lazyQueue.Value.TryDequeue(out message);


                //if the queue is empty, remove it from the dictionary
                if (lazyQueue.Value.IsEmpty)
                {
                    _processingMessages.TryRemove(_streamId, out var deletedQueue);
                    //The stream received new messages while we were removing it from the dictionary, so we need to re-add it
                    PreventNewMessagesRaceCondition(_streamId, deletedQueue);
                }

                break;
            }
            //The stream doesn't have messages anymore, but the current item was already purged
            case false when _current is not null:
                _onMessageRead.Invoke(null);
                break;
        }

        _current = message;
        

        return _current is not null;
    }

    private void PreventNewMessagesRaceCondition(StreamId streamId,
        Lazy<ConcurrentQueue<RabbitMqBatchContainer>> lazyQueue)
    {
        if (!lazyQueue.Value.IsEmpty)
        {
            _processingMessages.AddOrUpdate(streamId, _ => lazyQueue, (_, q) =>
            {
                while (q.Value.TryDequeue(out var batch))
                {
                    lazyQueue.Value.Enqueue(batch);
                }

                return lazyQueue;
            });
        }
    }

    public void Refresh(StreamSequenceToken token)
    {
        _processingMessages = _onPreRefresh();
        

        //Now that we have access again to the current processingMessages, we can just start reading messages again
        Initialize(true);
    }

    public void RecordDeliveryFailure()
    {
        if (_current is not null)
        {
            _current.DeliveryFailed = true;
        }
    }
}