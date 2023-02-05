using Grpc.Core;
using Newtonsoft.Json;

namespace SmartLock.Config.Models;

[GenerateSerializer]
public struct RpcExceptionSurrogate
{
    [Id(0)]
    public string Status { get; set; }
    [Id(1)]
    public string Trailers { get; set; }
    [Id(2)]
    public string Message { get; set; }
}

[RegisterConverter]
public class RpcExceptionSurrogateConverter : IConverter<RpcException, RpcExceptionSurrogate>
{
    static JsonSerializerSettings _settings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
    };
    public RpcException ConvertFromSurrogate(in RpcExceptionSurrogate surrogate)
    {
        var status = JsonConvert.DeserializeObject<Status>(surrogate.Status, _settings);
        var metadata = JsonConvert.DeserializeObject<Metadata>(surrogate.Trailers, _settings);
        return new RpcException(status, metadata, surrogate.Message);
    }

    public RpcExceptionSurrogate ConvertToSurrogate(in RpcException value)
    {
        var status = JsonConvert.SerializeObject(value.Status, _settings);
        var statusDetail = value.Message;
        var metadata = JsonConvert.SerializeObject(value.Trailers, _settings);
        
        return new RpcExceptionSurrogate()
        {
            Status = status,
            Trailers = metadata,
            Message = statusDetail,
        };
    }
}