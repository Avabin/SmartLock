using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Orleans.Serialization;
using Orleans.Streams;
using RabbitMQ.Stream.Client;
using SmartLock.Streams.RabbitMQ.Adapters;

namespace SmartLock.Streams.RabbitMQ.RabbitMQ;


public static class RabbitMQMessage
{
    public static string Format => "yyyyMMddHHmmssffff";
    public static string CreatedAtFieldName => "CreatedAt";
}

internal class RabbitMQQueueProvider
{
    private readonly RabbitMQStreamSystemProvider _streamSystemProvider;
    private readonly string _providerName;
    private readonly RabbitMQClientOptions _rabbitMqClientOptions;
    private readonly ConcurrentDictionary<QueueId, string> _queues = new();

    public RabbitMQQueueProvider(RabbitMQStreamSystemProvider streamSystemProvider, string providerName, RabbitMQClientOptions rabbitMqClientOptions)
    {
        _streamSystemProvider = streamSystemProvider;
        _providerName = providerName;
        _rabbitMqClientOptions = rabbitMqClientOptions;
    }

    public async Task<string> CreateOrGetQueue(QueueId queueId)
    {
        if (_queues.TryGetValue(queueId, out var queueName))
        {
            return queueName;
        }

        var streamSystem = await _streamSystemProvider.GetConsumerStream().ConfigureAwait(false);
        queueName = GetQueueName(queueId);
        await streamSystem.CreateStream(_rabbitMqClientOptions.StreamOptions with { Name = queueName }).ConfigureAwait(false);
        _queues.TryAdd(queueId, queueName);
        return queueName;
    }

    private string GetQueueName(QueueId queueId) =>
        $"{_providerName}-{queueId}";
}

/// <summary>
///     Client to manage RabbitMQ Streams consumer connections.
///     It uses RabbitMQ.Stream.Client underneath to connect to Rabbit Streams
/// </summary>
internal class RabbitMQConsumer
{
    private readonly ConcurrentQueue<RabbitMqBatchContainer> _batchContainers = new();
    private readonly ILogger<RabbitMQConsumer> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly QueueId _queueId;
    
    private readonly RabbitMQQueueProvider _rabbitMqQueueProvider;

    private readonly Serializer<RabbitMqBatchContainer> _serializer;
    private readonly RabbitMQStreamSystemProvider _streamSystemProvider;
    private IConsumer _consumer;
    
    
    private static readonly ActivitySource ActivitySource = new($"SmartLock.Streams.{nameof(RabbitMQConsumer)}");
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    private string _queueName;
    //private int _consumerState;
    //private const int StoppingState = 0;
    //private const int StartingState = 1;

    public RabbitMQConsumer(RabbitMQQueueProvider rabbitMqQueueProvider,
        RabbitMQStreamSystemProvider streamSystemProvider, ILoggerFactory loggerFactory, QueueId queueId,
        Serializer<RabbitMqBatchContainer> serializer)
    {
        _streamSystemProvider = streamSystemProvider;
        _loggerFactory = loggerFactory;
        _queueId = queueId;
        _serializer = serializer;
        _logger = loggerFactory.CreateLogger<RabbitMQConsumer>();
        _rabbitMqQueueProvider = rabbitMqQueueProvider;
    }

    public async Task CloseConsumer()
    {
        if (_consumer is null)
        {
            return;
        }

        _logger.LogInformation("Stopping reading from RabbitMQ queue {QueueName}", _queueName);

        try
        {
            //This Task.Delay is a temporary fix to prevent a deadlock due to RabbitMQ.Stream.Client taskObject.Result call :facepalm:
            //see: https://github.com/rabbitmq/rabbitmq-stream-dotnet-client/discussions/222
            await Task.Delay(1).ConfigureAwait(false);
            await _consumer.Close().ConfigureAwait(false);
            _batchContainers.Clear();
            _logger.LogInformation("{QueueName} consumer is not consuming messages anymore", _queueName);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to close consumer");
        }
    }

    public async Task StartConsumingMessages()
    {
        _queueName = await _rabbitMqQueueProvider.CreateOrGetQueue(_queueId).ConfigureAwait(false);
        await CreateConsumer((_, context, message) =>
            {
                var parentContext = Propagator.Extract(default, message.ApplicationProperties, (o, s) => o.TryGetValue(s, out var value) ? new []{ value?.ToString()} : null);
                Activity? activity = null;
                if (parentContext.ActivityContext.TraceId != default)
                {
                    _logger.LogDebug("Received message with parent context {ParentContext}", parentContext.ActivityContext.TraceId);
                    
                    var activityName = $"{nameof(RabbitMQConsumer)} {message.Data.Contents.Length} bytes";
                    Baggage.Current = parentContext.Baggage;
                    activity = ActivitySource.StartActivity(activityName, ActivityKind.Consumer, parentContext.ActivityContext);
                }
                message.ApplicationProperties.TryGetValue(RabbitMQMessage.CreatedAtFieldName, out var createdAt);
                var container = RabbitMqBatchContainer.FromRabbit(_serializer, message.Data.Contents, createdAt?.ToString(), context.Offset);
                _batchContainers.Enqueue(container);

                activity?.Stop();
                return Task.CompletedTask;
            })
            .ConfigureAwait(false);
    }

    public Task<IReadOnlyList<RabbitMqBatchContainer>> DequeueMessages(int maxCount)
    {
        
        var messages = new List<RabbitMqBatchContainer>(maxCount);

        while (messages.Count < maxCount && _batchContainers.TryDequeue(out var message))
        {
            messages.Add(message);
        }

        return Task.FromResult<IReadOnlyList<RabbitMqBatchContainer>>(messages);
    }

    public async Task UpdateOffset(ulong newOffset) => await _consumer.StoreOffset(newOffset).ConfigureAwait(false);

    private async Task<ulong> GetOffset()
    {
        var streamSystem = await _streamSystemProvider.GetConsumerStream().ConfigureAwait(false);
        _logger.LogInformation("Retrieving last offset for {Consumer} stream", _queueName);
        try
        {
            var initialOffset = await streamSystem.QueryOffset(_queueName, _queueName).ConfigureAwait(false);

            _logger.LogInformation("The {QueueName} consumer will resume consuming from message offset {Offset}",
                _queueName, initialOffset);
            return initialOffset;
        }
        catch
        {
            _logger.LogInformation("There is no offset for {StreamName} yet, will start consuming from 0", _queueName);
            return 0;
        }
    }

    private async Task CreateConsumer(Func<RawConsumer, MessageContext, Message, Task> onNewMessage)
    {
        _logger.LogInformation("Creating consumer for {QueueName}", _queueName);
        var streamSystem = await _streamSystemProvider.GetConsumerStream().ConfigureAwait(false);

        var initialOffset = await GetOffset().ConfigureAwait(false);
        _consumer = await streamSystem.CreateRawConsumer(
                new RawConsumerConfig(_queueName)
                {
                    OffsetSpec = new OffsetTypeOffset(initialOffset),
                    Reference = _queueName,
                    MessageHandler = onNewMessage,
                    IsSingleActiveConsumer = true,
                    MetadataHandler = update => { _logger.LogInformation($"Update Code: {update.Code}"); }
                }, _loggerFactory.CreateLogger<IConsumer>()).ConfigureAwait(false);
        _logger.LogInformation($"Consumer created, now consuming {_queueName}");
    }
}