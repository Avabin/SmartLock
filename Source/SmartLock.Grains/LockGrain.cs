using SmartLock.GrainInterfaces;

namespace SmartLock.Grains;

public class LockGrain : Grain<LockGrainState>, ILockGrain
{
    public async ValueTask<bool> IsLockedAsync() => State.IsLocked;

    public async ValueTask LockAsync()
    {
        State.Lock();
        await WriteStateAsync();
    }

    public async ValueTask UnlockAsync()
    {
        State.Unlock();
        await WriteStateAsync();
    }
}

[GenerateSerializer]
public record LockGrainState
{
    [Id(0)]
    public bool IsLocked { get; set; } = false;
    
    public void Lock() => IsLocked = true;

    public void Unlock() => IsLocked = false;
}