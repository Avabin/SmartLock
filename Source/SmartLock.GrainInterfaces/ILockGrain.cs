namespace SmartLock.GrainInterfaces;

public interface ILockGrain : IGrainWithStringKey
{
    ValueTask<bool> IsLockedAsync();
    ValueTask LockAsync();
    ValueTask UnlockAsync();
}