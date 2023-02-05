using SmartLock.Client.Models;

namespace SmartLock.Client.HttpClient;

public interface IClientSettingsService
{
    Task<ClientSettings> GetClientSettingsAsync(string deviceId);

    Task RegisterAsync(string deviceId, string name, string? defaultBuilding = null);
}

internal class ClientSettingsService : IClientSettingsService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ClientSettingsService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task<ClientSettings> GetClientSettingsAsync(string deviceId)
    {
        var client = _httpClientFactory.CreateClient(HttpClientConstants.Name);
        
        return await client.GetClientSettingsAsync(deviceId);
    }

    public async Task RegisterAsync(string deviceId, string name, string? defaultBuilding = null)
    {
        var client = _httpClientFactory.CreateClient(HttpClientConstants.Name);
        
        await client.RegisterClientAsync(deviceId, name, defaultBuilding);
    }
}