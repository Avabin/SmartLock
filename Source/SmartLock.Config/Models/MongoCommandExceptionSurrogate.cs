using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Connections;
using Newtonsoft.Json;

namespace SmartLock.Config.Models;

[GenerateSerializer]
public struct MongoCommandExceptionSurrogate
{
    [Id(0)]
    public string Command;
    [Id(1)]
    public string Result;
    [Id(2)]
    public string ConnectionId;
    [Id(3)]
    public string Message;
}

[RegisterConverter]
public class MongoCommandExceptionSurrogateConverter : IConverter<MongoCommandException, MongoCommandExceptionSurrogate>
{
    private static JsonSerializerSettings _settings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
    };
    public MongoCommandException ConvertFromSurrogate(in MongoCommandExceptionSurrogate surrogate)
    {
        var command = BsonDocument.Parse(surrogate.Command);
        var result = BsonDocument.Parse(surrogate.Result);
        var connectionId = JsonConvert.DeserializeObject<ConnectionId>(surrogate.ConnectionId, _settings);
        
        return new MongoCommandException(connectionId, surrogate.Message, command, result);
    }

    public MongoCommandExceptionSurrogate ConvertToSurrogate(in MongoCommandException value)
    {
        var command = value.Command.ToJson();
        var result = value.Result.ToJson();
        var connectionId = JsonConvert.SerializeObject(value.ConnectionId, _settings);
        var message = value.Message;
        return new MongoCommandExceptionSurrogate()
        {
            Command = command,
            Result = result,
            ConnectionId = connectionId,
            Message = message,
        };
    }
}