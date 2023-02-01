using SmartLock.Client.Models;

namespace SmartLock.GrainInterfaces;

public interface IBuildingGrain : IGrainWithStringKey
{
    /// <summary>
    /// Gets the building lock by its location.
    /// </summary>
    /// <param name="locationModel">The lock location.</param>
    /// <returns></returns>
    ValueTask<ILockGrain?> GetLockAsync(LocationModel locationModel);
    
    /// <summary>
    /// Adds a new lock to the building.
    /// </summary>
    /// <param name="locationModel">The lock location.</param>
    /// <returns>Awaitable task</returns>
    ValueTask CreateLockAsync(LocationModel locationModel);
    
    /// <summary>
    /// Removes a lock from the building.
    /// </summary>
    /// <param name="locationModel">The lock location.</param>
    /// <returns>Awaitable task</returns>
    ValueTask DeleteLockAsync(LocationModel locationModel);
}