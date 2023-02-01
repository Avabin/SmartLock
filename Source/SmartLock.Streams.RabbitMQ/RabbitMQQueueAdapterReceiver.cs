using System.Reactive.Disposables;
using System.Text;
using Microsoft.Extensions.Logging;
using Orleans.Providers.Streams.Common;
using Orleans.Runtime;
using Orleans.Streams;
using RabbitMQ.Client;

namespace SmartLock.Streams.RabbitMQ;

public class RabbitMQQueueAdapterReceiver : IQueueAdapterReceiver, IDisposable
{
    private readonly Lazy<RabbitMQObservableModel> _channel;
    private readonly IConnection                   _connection;
    private readonly ILogger<RabbitMQQueueAdapterReceiver> _logger;
    private readonly CompositeDisposable           _disposables = new();

    public RabbitMQQueueAdapterReceiver(IConnection connection, QueueId queueId,
        ILogger<RabbitMQQueueAdapterReceiver> logger)
    {
        _connection = connection;
        _logger = logger;

        _channel = new Lazy<RabbitMQObservableModel>(() =>
        {
            var channel = _connection.CreateModel();
            var queue = queueId.ToString();
            _logger.LogTrace("Declaring exchange 'orleans'");
            channel.ExchangeDeclare("orleans", ExchangeType.Topic, true);
            _logger.LogTrace("Declaring queue '{Queue}'", queue);
            channel.QueueDeclare(queue, true, false, false);
            _logger.LogTrace("Binding queue '{Queue}' to exchange 'orleans' with routing key '#'", queue);
            channel.QueueBind(queue, "orleans", "#");
            return new RabbitMQObservableModel(channel, queueId);
        });
    }

    private RabbitMQObservableModel Channel => _channel.Value;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public Task Initialize(TimeSpan timeout)
    {
        var sub = Channel.Subscribe();
        _disposables.Add(sub);
        return Task.CompletedTask;
    }

    public Task<IList<IBatchContainer>> GetQueueMessagesAsync(int maxCount)
    {
        var msgs = Channel.Get(maxCount)
                          .GroupBy(x => x.BasicProperties.CorrelationId)
                          .Select((x, i) =>
                           {
                               var streamId = StreamId.Parse(Encoding.UTF8.GetBytes(x.Key));
                               return new RabbitMQBatchContainer(streamId, new EventSequenceTokenV2(i),
                                                                 x.AsEnumerable().Select(Delivery.From));
                           }).Cast<IBatchContainer>()
                          .ToList()
                          .AsReadOnly();

        return Task.FromResult<IList<IBatchContainer>>(msgs);
    }

    public async Task MessagesDeliveredAsync(IList<IBatchContainer> messages)
    {
        await Task.Run(() =>
        {
            var channel = Channel;

            foreach (var message in messages)
            {
                var batch             = (RabbitMQBatchContainer)message;
                var latestDeliveryTag = batch.GetLatestDeliveryTag();
                channel.Model.BasicAck((ulong)latestDeliveryTag, true);
            }
        });
    }

    public Task Shutdown(TimeSpan timeout)
    {
        Channel.Model.Close();
        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _connection.Dispose();
        _disposables.Dispose();
    }
}