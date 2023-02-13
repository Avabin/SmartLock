using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Orleans.Streams;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.AMQP;
using RabbitMQ.Stream.Client.Reliable;

namespace SmartLock.Streams.RabbitMQ.RabbitMQ;

internal class RabbitMQProducer : IAsyncDisposable
{
    private readonly RabbitMQStreamSystemProvider _streamSystemProvider;
    private readonly RabbitMQQueueProvider _rabbitMqQueueProvider;
    private readonly QueueId _queueId;
    private Producer _producer;
    private object _lock = new();
    private Task<Producer> _producerCreatingTask;
    private static ActivitySource _activitySource = new($"SmartLock.Streams.{nameof(RabbitMQProducer)}");
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    public RabbitMQProducer(RabbitMQStreamSystemProvider streamSystemProvider, RabbitMQQueueProvider rabbitMqQueueProvider, QueueId queueId)
    {
        _streamSystemProvider = streamSystemProvider;
        _rabbitMqQueueProvider = rabbitMqQueueProvider;
        _queueId = queueId;
    }

    public async Task SendMessage(byte[] messageBody)
    {
        var activityName = $"{nameof(RabbitMQProducer)}.{nameof(SendMessage)}";
        using var activity = _activitySource.StartActivity(activityName, ActivityKind.Producer);
        activity?.SetIdFormat(ActivityIdFormat.W3C);
        activity?.AddTag("rpc.system", "rabbitmq");
        activity?.AddTag("rpc.service", "SmartLock.Streams.RabbitMQ.RabbitMQProducer");
        activity?.AddTag("rpc.method", "SendMessage");
        activity?.AddTag("rpc.queue", _queueId.ToString());
        var producer = await GetProducer().ConfigureAwait(false);
        var message = new Message(messageBody)
        {
            ApplicationProperties =
                new ApplicationProperties
                {
                    { RabbitMQMessage.CreatedAtFieldName, DateTime.UtcNow.ToString(RabbitMQMessage.Format) }
                }
        };
        // Depending on Sampling (and whether a listener is registered or not), the
        // activity above may not be created.
        // If it is created, then propagate its context.
        // If it is not created, the propagate the Current context,
        // if any.
        ActivityContext contextToInject = default;
        if (activity != null)
        {
            contextToInject = activity.Context;
        }
        else if (Activity.Current != null)
        {
            contextToInject = Activity.Current.Context;
        }
        
        Propagator.Inject(new PropagationContext(contextToInject, Baggage.Current), message, (m, k, v) => m.ApplicationProperties.TryAdd(k, v));

        await producer.Send(message).ConfigureAwait(false);
    }

    private async Task<Producer> GetProducer()
    {
        if (_producer is not null)
        {
            return _producer;
        }

        lock (_lock)
        {
            _producerCreatingTask ??= CreateProducer();
        }

        return await _producerCreatingTask.ConfigureAwait(false);
    }

    private async Task<Producer> CreateProducer()
    {
        var streamSystem = await _streamSystemProvider.GetConsumerStream().ConfigureAwait(false);
        var queueName = await _rabbitMqQueueProvider.CreateOrGetQueue(_queueId).ConfigureAwait(false);
        _producer = await Producer.Create(new ProducerConfig(streamSystem, queueName)).ConfigureAwait(false);

        return _producer;
    }

    public async ValueTask DisposeAsync()
    {
        _producerCreatingTask?.Dispose();

        if (_producer is not null)
            await _producer.Close().ConfigureAwait(false);
    }
}