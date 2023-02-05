using FluentAssertions;
using Orleans.TestingHost;
using SmartLock.Client.Models;
using SmartLock.GrainInterfaces;
using Tests.Shared;

namespace SmartLock.Grains.Tests.Grains.JournaledBuildingGrain;

[Parallelizable(ParallelScope.Children)]
public abstract class JournaledBuildingGrainTestsBase<T, TClient> : GrainTestBase<T, TClient> where T : ISiloConfigurator, new() where TClient : IClientBuilderConfigurator, new()
{
    [Test]
    public async Task When_AddLock_LockAdded()
    {
        // Arrange
        var locationRandomName = Guid.NewGuid().ToString("N");
        var buildingLocation = new LocationModel(locationRandomName);
        var lockLocation = new LocationModel($"Front Door-{locationRandomName}");
        var grain = this.GrainFactory.GetGrain<IJournaledBuildingGrain>(buildingLocation.Value);
        // Act
        await grain.AddLockAsync(lockLocation);
        var @lock = await grain.GetLockAsync(lockLocation);
        
        // Assert
        @lock?.Location.Should().NotBeNull();
        @lock.Value.Location.Should().Be(lockLocation);
    }
    [Test]
    public async Task When_AddLock_DuplicateLock_Exists_OnlyOneAdded()
    {
        // Arrange
        var locationRandomName = Guid.NewGuid().ToString("N");
        var buildingLocation = new LocationModel(locationRandomName);
        var lockLocation = new LocationModel($"Front Door-{locationRandomName}");
        var grain = this.GrainFactory.GetGrain<IJournaledBuildingGrain>(buildingLocation.Value);
        await grain.AddLockAsync(lockLocation);
        await grain.AddLockAsync(lockLocation);
        // Act
        var locks = await grain.GetLocksAsync();
        
        // Assert
        locks.Should().HaveCount(1);
        locks.First().Location.Should().Be(lockLocation);
    }
    
    [Test]
    public async Task When_UnlockAllAsync_AllLocksUnlocked()
    {
        // Arrange
        var locationRandomName = Guid.NewGuid().ToString("N");
        var buildingLocation = new LocationModel(locationRandomName);
        var grain = this.GrainFactory.GetGrain<IJournaledBuildingGrain>(buildingLocation.Value);
        var lockLocations = Enumerable.Range(1, 10).Select(i => new LocationModel($"Lock {i}-{locationRandomName}")).ToList();
        await Task.WhenAll(lockLocations.Select(l => grain.AddLockAsync(l).AsTask()));
        await grain.LockAllAsync();

        // Act
        await grain.UnlockAllAsync();
        var locks = await grain.GetLocksAsync();
        
        // Assert
        locks.Count.Should().Be(10);
        locks.Select(x => x.IsOpen).Should().OnlyContain(x => x == true);
    }
    
    //create test for LockAllAsync using 10 locks
    [Test]
    public async Task When_LockAllAsync_AllLocksLocked()
    {
        // Arrange
        var locationRandomName = Guid.NewGuid().ToString("N");
        var buildingLocation = new LocationModel(locationRandomName);
        var grain = this.GrainFactory.GetGrain<IJournaledBuildingGrain>(buildingLocation.Value);
        var lockLocations = Enumerable.Range(1, 10).Select(i => new LocationModel($"Lock {i}{i}-{locationRandomName}")).ToList();
        await Task.WhenAll(lockLocations.Select(l => grain.AddLockAsync(l).AsTask()));

        // Act
        await grain.LockAllAsync();
        var models = await grain.GetLocksAsync();
        
        // Assert
        models.Count.Should().Be(10);
        models.Select(x => x.IsOpen).Should().OnlyContain(x => x == false);
    }
    
    //create test for RemoveLockAsync using 1 lock
    [Test]
    public async Task When_RemoveLockAsync_LockRemoved()
    {
        // Arrange
        var locationRandomName = Guid.NewGuid().ToString("N");
        var buildingLocation = new LocationModel(locationRandomName);
        var lockLocation = new LocationModel("Front Door-" + locationRandomName);
        var grain = this.GrainFactory.GetGrain<IJournaledBuildingGrain>(buildingLocation.Value);
        await grain.AddLockAsync(lockLocation);

        // Act
        await grain.RemoveLockAsync(lockLocation);

        // Assert
        var @lock = await grain.GetLockAsync(lockLocation);
        @lock.Should().BeNull();
    }
    
    //create test for UnlockAsync using 1 lock
    [Test]
    public async Task When_UnlockAsync_LockUnlocked()
    {
        // Arrange
        var locationRandomName = Guid.NewGuid().ToString("N");
        var buildingLocation = new LocationModel(locationRandomName);
        var lockLocation = new LocationModel($"Front Door-{locationRandomName}");
        var grain = this.GrainFactory.GetGrain<IJournaledBuildingGrain>(buildingLocation.Value);
        await grain.AddLockAsync(lockLocation);
        await grain.LockAllAsync();

        // Act
        await grain.UnlockAsync(lockLocation);
        var @lock = await grain.GetLockAsync(lockLocation);

        // Assert
        @lock.Should().NotBeNull();
        @lock.Value.IsOpen.Should().BeTrue();
    }
    
    //create test for LockAsync using 1 lock
    [Test, Parallelizable]
    public async Task When_LockAsync_LockLocked()
    {
        // Arrange
        var locationRandomName = Guid.NewGuid().ToString("N");
        var buildingLocation = new LocationModel(locationRandomName);
        var lockLocation = new LocationModel($"Front Door-{locationRandomName}");
        var grain = this.GrainFactory.GetGrain<IJournaledBuildingGrain>(buildingLocation.Value);
        await grain.AddLockAsync(lockLocation);

        // Act
        await grain.LockAsync(lockLocation);
        var isLocked = await grain.GetLockAsync(lockLocation);

        // Assert
        isLocked.Should().NotBeNull();
        isLocked.Value.IsOpen.Should().BeFalse();
    }

}