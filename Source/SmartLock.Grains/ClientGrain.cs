using SmartLock.Client.Models;
using SmartLock.GrainInterfaces;

namespace SmartLock.Grains;

public class ClientGrain : Grain<ClientGrainState>, IClientGrain
{
    public ValueTask<ClientSettings> GetSettingsAsync() => new(new ClientSettings(new LocationModel(State.DefaultBuilding), State.Name, State.DeviceId));
    public async ValueTask RegisterAsync(RegisterClientModel model)
    {
        State.DeviceId = model.DeviceId;
        State.Name = model.Name;
        State.DefaultBuilding = model.DefaultBuilding ?? "";
        await WriteStateAsync();
    }
}

[GenerateSerializer]
public class ClientGrainState
{
    [Id(0)]
    public string DeviceId { get; set; } = "";
    [Id(1)]
    public string Name { get; set; } = "";

    [Id(2)] public string DefaultBuilding { get; set; } = "";
}