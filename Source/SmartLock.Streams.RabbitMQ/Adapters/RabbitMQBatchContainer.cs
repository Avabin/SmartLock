using System.Buffers;
using Orleans.Providers.Streams.Common;
using Orleans.Runtime;
using Orleans.Serialization;
using Orleans.Streams;

namespace SmartLock.Streams.RabbitMQ.Adapters;

[GenerateSerializer]
public record RabbitMqBatchContainer : IBatchContainer
{
    [Id(0)] private readonly List<object> _events;

    [Id(1)] private readonly Dictionary<string, object> _requestContext;

    [Id(2)] private EventSequenceTokenV2 _sequenceTokenV2;

    private RabbitMqBatchContainer(StreamId streamId, List<object> events, Dictionary<string, object> requestContext)
    {
        StreamId = streamId;
        _events = events;
        _requestContext = requestContext;
    }

    [field: NonSerialized] public bool DeliveryFailed { get; set; }

    [field: NonSerialized] public string CreatedAt { get; private set; }

    [Id(3)] public StreamId StreamId { get; }

    public IEnumerable<Tuple<T, StreamSequenceToken>> GetEvents<T>() => _events.OfType<T>().Select((e, i) =>
        Tuple.Create<T, StreamSequenceToken>(e, _sequenceTokenV2.CreateSequenceTokenForEvent(i)));

    public StreamSequenceToken SequenceToken => _sequenceTokenV2;

    public bool ImportRequestContext()
    {
        if (_requestContext is null)
        {
            return false;
        }

        RequestContextExtensions.Import(_requestContext);
        return true;
    }

    public static RabbitMqBatchContainer FromRabbit(Serializer<RabbitMqBatchContainer> serializer,
        ReadOnlySequence<byte> messageBody, string createdAt, ulong offset)
    {
        var batchContainer = serializer.Deserialize(messageBody);
        batchContainer._sequenceTokenV2 = new EventSequenceTokenV2(Convert.ToInt64(offset));
        batchContainer.CreatedAt = createdAt;

        return batchContainer;
    }


    public static byte[] ToRabbitMqMessage<T>(Serializer<RabbitMqBatchContainer> serializer,
        StreamId streamId,
        IEnumerable<T> events, Dictionary<string, object> requestContext
    )
    {
        var batchContainer = new RabbitMqBatchContainer(streamId, events.Cast<object>().ToList(), requestContext);
        return serializer.SerializeToArray(batchContainer);
    }
}