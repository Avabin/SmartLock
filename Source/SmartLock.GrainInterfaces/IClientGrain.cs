using SmartLock.Client.Models;

namespace SmartLock.GrainInterfaces;

public interface IClientGrain : IGrainWithStringKey
{
    ValueTask<ClientSettings> GetSettingsAsync();
    ValueTask RegisterAsync(RegisterClientModel model);
}