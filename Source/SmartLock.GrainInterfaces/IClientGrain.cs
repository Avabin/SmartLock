using Orleans.Concurrency;
using SmartLock.Client.Models;

namespace SmartLock.GrainInterfaces;

public interface IClientGrain : IGrainWithStringKey
{
    [ReadOnly]
    ValueTask<ClientSettings> GetSettingsAsync();
    ValueTask RegisterAsync(RegisterClientModel model);
}