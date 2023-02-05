using System.Net;
using System.Net.Http.Json;
using SmartLock.Client.Models;

namespace SmartLock.Client.HttpClient;

internal static class HttpClientExtensions
{
    #region ClientController
    public static async Task RegisterClientAsync(this System.Net.Http.HttpClient client, string deviceId, string name, string? defaultBuilding = null)
    {
        var model = new RegisterClientModel(deviceId, name, defaultBuilding);
        var responseMessage = await client.PostAsJsonAsync("api/Client/register", model);
        
        responseMessage.EnsureSuccessStatusCode();
    }
    
    public static async Task<ClientSettings> GetClientSettingsAsync(this System.Net.Http.HttpClient client, string deviceId)
    {
        var responseMessage = await client.GetAsync($"api/Client/settings?deviceId={deviceId}");
        
        responseMessage.EnsureSuccessStatusCode();
        
        return await responseMessage.Content.ReadFromJsonAsync<ClientSettings>()!;
    }
    #endregion
    
    #region BuildingsController
    
    // api/Buildings/locks
    public static async Task<IReadOnlyList<LockModel>> GetLocksAsync(this System.Net.Http.HttpClient client, string building)
    {
        var responseMessage = await client.GetAsync($"api/Buildings/locks?building={building}");
        
        responseMessage.EnsureSuccessStatusCode();
        
        return await responseMessage.Content.ReadFromJsonAsync<IReadOnlyList<LockModel>>() ?? Array.Empty<LockModel>();
    }
    
    // api/Buildings/lock/status
    public static async Task<bool> GetLockStatusAsync(this System.Net.Http.HttpClient client, string building, string lockLocation)
    {
        var responseMessage = await client.GetAsync($"api/Buildings/lock/status?building={building}&lockLocation={lockLocation}");
        
        responseMessage.EnsureSuccessStatusCode();
        
        return await responseMessage.Content.ReadFromJsonAsync<bool>();
    }
    
    // api/Buildings/locks/status
    public static async Task<IReadOnlyList<LockModel>> GetLocksStatusAsync(this System.Net.Http.HttpClient client, string building, params string[] locks)
    {
        var responseMessage = await client.PostAsJsonAsync($"api/Buildings/locks/status?building={building}", locks);
        
        responseMessage.EnsureSuccessStatusCode();
        return await responseMessage.Content.ReadFromJsonAsync<IReadOnlyList<LockModel>>() ?? Array.Empty<LockModel>();
    }
    
    // api/Buildings/openLock
    public static async Task OpenLockAsync(this System.Net.Http.HttpClient client, string building, string lockLocation)
    {
        var responseMessage = await client.PostAsync($"api/Buildings/openLock?building={building}&lockLocation={lockLocation}", null);
        
        responseMessage.EnsureSuccessStatusCode();
    }
    
    // api/Buildings/closeLock
    public static async Task CloseLockAsync(this System.Net.Http.HttpClient client, string building, string lockLocation)
    {
        var responseMessage = await client.PostAsync($"api/Buildings/closeLock?building={building}&lockLocation={lockLocation}", null);

        responseMessage.EnsureSuccessStatusCode();
    }
    
    // api/Buildings/addLock
    public static async Task AddLockAsync(this System.Net.Http.HttpClient client, string building, string lockLocation)
    {
        var data = new  {building, lockLocation};
        var responseMessage = await client.PostAsJsonAsync($"api/Buildings/addLock", data);
        
        responseMessage.EnsureSuccessStatusCode();
    }
    
    // api/Buildings/removeLock
    public static async Task RemoveLockAsync(this System.Net.Http.HttpClient client, string building, string lockLocation)
    {
        var responseMessage = await client.DeleteAsync($"api/Buildings/removeLock?building={building}&lockLocation={lockLocation}");
        
        responseMessage.EnsureSuccessStatusCode();
    }
    
    // api/Buildings/openAll
    public static async Task OpenAllLocksAsync(this System.Net.Http.HttpClient client, string building)
    {
        var responseMessage = await client.PostAsync($"api/Buildings/openAll?building={building}", null);
        
        responseMessage.EnsureSuccessStatusCode();
    }
    
    // api/Buildings/closeAll
    public static async Task CloseAllLocksAsync(this System.Net.Http.HttpClient client, string building)
    {
        var responseMessage = await client.PostAsync($"api/Buildings/closeAll?building={building}", null);
        
        responseMessage.EnsureSuccessStatusCode();
    }

    #endregion
    
    #region DetectionController
    
    // api/Detection/detect with image url
    public static async Task<IReadOnlyList<DetectedObjectModel>> DetectAsync(this System.Net.Http.HttpClient client, string imageUrl)
    {
        var responseMessage = await client.GetAsync($"api/Detection/detect?imageUrl={imageUrl}");
        
        if (!responseMessage.IsSuccessStatusCode)
        {
            var error = await responseMessage.Content.ReadAsStringAsync();
            throw new HttpRequestException(error);
        }
        
        return await responseMessage.Content.ReadFromJsonAsync<IReadOnlyList<DetectedObjectModel>>()!;
    }
    
    // api/Detection/detect with image stream as multipart form data
    public static async Task<IReadOnlyList<DetectedObjectModel>> DetectAsync(this System.Net.Http.HttpClient client, Stream imageStream)
    {
        var content = new MultipartFormDataContent();
        content.Add(new StreamContent(imageStream), "file", "image.jpg");
        
        var responseMessage = await client.PostAsync("api/Detection/detect", content);

        if (!responseMessage.IsSuccessStatusCode)
        {
            var error = await responseMessage.Content.ReadAsStringAsync();
            throw new HttpRequestException(error);
        }
        
        return await responseMessage.Content.ReadFromJsonAsync<IReadOnlyList<DetectedObjectModel>>() ?? Array.Empty<DetectedObjectModel>();
    }
    
    #endregion
}