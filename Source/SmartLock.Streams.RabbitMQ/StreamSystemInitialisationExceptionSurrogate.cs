using RabbitMQ.Stream.Client;

namespace SmartLock.Streams.RabbitMQ;

[GenerateSerializer]
public struct StreamSystemInitialisationExceptionSurrogate
{
    [Id(0)]
    public string Reason;
}

[RegisterConverter]
public sealed class StreamSystemInitialisationExceptionSurrogateConverter : IConverter<StreamSystemInitialisationException, StreamSystemInitialisationExceptionSurrogate>
{
    public StreamSystemInitialisationException ConvertFromSurrogate(in StreamSystemInitialisationExceptionSurrogate surrogate) => new(surrogate.Reason);

    public StreamSystemInitialisationExceptionSurrogate ConvertToSurrogate(in StreamSystemInitialisationException value) => new()
        {
            Reason = value.Message
        };
}