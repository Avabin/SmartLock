using System.Runtime.CompilerServices;
using SmartLock.Client.Models;

[assembly: InternalsVisibleTo("SmartLock.Grains.Tests")]
namespace SmartLock.Client.HttpClient;

internal class BuildingsService : IBuildingsService
{
    private System.Net.Http.HttpClient _httpClient;

    public BuildingsService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConstants.Name);
    }

    // api/Buildings/locks
    public async ValueTask<IReadOnlyList<LockModel>> GetLocksAsync(LocationModel building)
    {
        var models = await _httpClient.GetLocksAsync(building);
        
        return models;
    }

    // api/Buildings/lock/status
    public async ValueTask<LockModel> GetLockAsync(LocationModel lockLocation, LocationModel building)
    {
        var status = await _httpClient.GetLockStatusAsync(building, lockLocation);
        
        return new LockModel(lockLocation, status);
    }
    
    // api/Buildings/locks/status
    public async ValueTask<IReadOnlyList<LockModel>> GetLocksAsync(LocationModel building, params LocationModel[] locks)
    {
        var models = await _httpClient.GetLocksStatusAsync(building, locks.Select(x => x.Value).ToArray());
        
        return models;
    }
    
    // api/Buildings/openLock
    public async ValueTask OpenLockAsync(LocationModel lockLocation, LocationModel building)
    {
        await _httpClient.OpenLockAsync(building, lockLocation);
    }
    
    // api/Buildings/closeLock
    public async ValueTask CloseLockAsync(LocationModel lockLocation, LocationModel building)
    {
        await _httpClient.CloseLockAsync(building, lockLocation);
    }
    
    // api/Buildings/addLock
    public async ValueTask AddLockAsync(LocationModel lockLocation, LocationModel building)
    {
        await _httpClient.AddLockAsync(building, lockLocation);
    }
    
    // api/Buildings/removeLock
    public async ValueTask RemoveLockAsync(LocationModel lockLocation, LocationModel building)
    {
        await _httpClient.RemoveLockAsync(building, lockLocation);
    }
    
    // api/Buildings/openAll
    public async ValueTask OpenAllLocksAsync(LocationModel building)
    {
        await _httpClient.OpenAllLocksAsync(building);
    }
    
    // api/Buildings/closeAll
    public async ValueTask CloseAllLocksAsync(LocationModel building)
    {
        await _httpClient.CloseAllLocksAsync(building);
    }
}