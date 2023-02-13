using Orleans.Concurrency;
using SmartLock.Client.Models;

namespace SmartLock.GrainInterfaces;

public interface IJournaledBuildingGrain : IGrainWithStringKey
{
    ValueTask AddLockAsync(LocationModel location);
    ValueTask RemoveLockAsync(LocationModel location);
    [ReadOnly]
    ValueTask<LockModel?> GetLockAsync(LocationModel location);

    ValueTask UnlockAsync(LocationModel location);
    
    ValueTask LockAsync(LocationModel location);

    ValueTask UnlockAllAsync();
    
    ValueTask LockAllAsync();
    [AlwaysInterleave]
    Task<LocationModel> GetBuildingLocationAsync();
    [ReadOnly]
    Task<IReadOnlyList<LockModel>> GetLocksAsync();
    [ReadOnly]
    Task<IReadOnlyList<LockModel>> GetLocksAsync(params LocationModel[] locks);
}