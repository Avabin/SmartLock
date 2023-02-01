using SmartLock.Client.Models;

namespace SmartLock.Client.HttpClient;

public interface IBuildingsService
{
    ValueTask<IReadOnlyList<LockModel>> GetLocksAsync(LocationModel building);
    ValueTask<LockModel> GetLockAsync(LocationModel lockLocation, LocationModel building);
    ValueTask<IReadOnlyList<LockModel>> GetLocksAsync(LocationModel building, params LocationModel[] locks);
    ValueTask OpenLockAsync(LocationModel lockLocation, LocationModel building);
    ValueTask CloseLockAsync(LocationModel lockLocation, LocationModel building);
    ValueTask AddLockAsync(LocationModel lockLocation, LocationModel building);
    ValueTask RemoveLockAsync(LocationModel lockLocation, LocationModel building);
    ValueTask OpenAllLocksAsync(LocationModel building);
    ValueTask CloseAllLocksAsync(LocationModel building);
}