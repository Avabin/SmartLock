using System.Collections.Immutable;
using SmartLock.Client.Models;
using SmartLock.GrainInterfaces;

namespace SmartLock.Grains;

public class BuildingGrain : Grain<BuildingGrainState>, IBuildingGrain
{
    public async ValueTask<ILockGrain?> GetLockAsync(LocationModel locationModel) =>
        State.Locks.TryGetValue(locationModel, out var lockId) 
            ? GrainFactory.GetGrain<ILockGrain>(lockId.Value) 
            : null;

    public async ValueTask CreateLockAsync(LocationModel locationModel)
    {
        State.AddLock(locationModel);
        await WriteStateAsync();
    }
    
    public async ValueTask DeleteLockAsync(LocationModel locationModel)
    {
        State.RemoveLock(locationModel);
        await WriteStateAsync();
    }
}

[GenerateSerializer]
public record BuildingGrainState
{
    [Id(0)]
    public ImmutableHashSet<LocationModel> Locks { get; set; } = ImmutableHashSet<LocationModel>.Empty; 
    
    /// <summary>
    /// Add a lock to the building.
    /// </summary>
    /// <param name="locationModel">The location of the lock.</param>
    public void AddLock(LocationModel locationModel) => Locks = Locks.Add(locationModel);
    
    /// <summary>
    /// Removes a lock from the building.
    /// </summary>
    /// <param name="locationModel">The location of the lock to remove.</param>
    public void RemoveLock(LocationModel locationModel) => Locks = Locks.Remove(locationModel);
}