using SmartLock.Client.Models;

namespace SmartLock.GrainInterfaces;

public interface IJournaledBuildingGrain : IGrainWithStringKey
{
    ValueTask AddLockAsync(LocationModel location);
    ValueTask RemoveLockAsync(LocationModel location);
    ValueTask<LockModel?> GetLockAsync(LocationModel location);

    ValueTask UnlockAsync(LocationModel location);
    
    ValueTask LockAsync(LocationModel location);

    ValueTask UnlockAllAsync();
    
    ValueTask LockAllAsync();
    Task<LocationModel> GetBuildingLocationAsync();
    Task<IReadOnlyList<LockModel>> GetLocksAsync();
    Task<IReadOnlyList<LockModel>> GetLocksAsync(params LocationModel[] locks);
}