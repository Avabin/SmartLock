using FluentAssertions;
using Orleans.TestingHost;
using SmartLock.Client.Models;
using SmartLock.GrainInterfaces;
using Tests.Shared;

namespace SmartLock.Grains.Tests.Grains.BuildingGrain;

public class BuildingGrainTestsBase<TSiloConfigurator, TClientConfigurator> : GrainTestBase<TSiloConfigurator, TClientConfigurator>
    where TSiloConfigurator : ISiloConfigurator, new()
    where TClientConfigurator : IClientBuilderConfigurator, new()
{
    [Test]
    public async Task When_CreateLock_LockCreated()
    {
        // Arrange
        var key = Guid.NewGuid().ToString("N");
        var location = new LocationModel(key);
        var buildingKey = Guid.NewGuid().ToString("N");
        var grain = this.GrainFactory.GetGrain<IBuildingGrain>(buildingKey);
        
        // Act
        await grain.CreateLockAsync(location);
        var @lock = await grain.GetLockAsync(location);
        
        // Assert
        @lock.Should().NotBeNull();
        @lock.GetPrimaryKeyString().Should().Be(location.Value);
    }
    
    [Test]
    public async Task When_GetLockAsync_LockDoesNotExist_ReturnsNull()
    {
        // Arrange
        var key = Guid.NewGuid().ToString("N");
        var location = new LocationModel(key);
        var buildingKey = Guid.NewGuid().ToString("N");
        var grain = this.GrainFactory.GetGrain<IBuildingGrain>(buildingKey);
        
        // Act
        var @lock = await grain.GetLockAsync(location);
        
        // Assert
        @lock.Should().BeNull();
    }
    
    [Test]
    public async Task When_GetLockAsync_LockExists_ReturnsLock()
    {
        // Arrange
        var key = Guid.NewGuid().ToString("N");
        var location = new LocationModel(key);
        var buildingKey = Guid.NewGuid().ToString("N");
        var grain = this.GrainFactory.GetGrain<IBuildingGrain>(buildingKey);
        await grain.CreateLockAsync(location);
        
        // Act
        var @lock = await grain.GetLockAsync(location);
        
        // Assert
        @lock.Should().NotBeNull();
        @lock.GetPrimaryKeyString().Should().Be(location.Value);
    }
    
    [Test]
    public async Task When_GetLockAsync_LockExists_ReturnsSameLock()
    {
        // Arrange
        var key = Guid.NewGuid().ToString("N");
        var location = new LocationModel(key);
        var buildingKey = Guid.NewGuid().ToString("N");
        var grain = this.GrainFactory.GetGrain<IBuildingGrain>(buildingKey);
        await grain.CreateLockAsync(location);
        
        // Act
        var @lock1 = await grain.GetLockAsync(location);
        var @lock2 = await grain.GetLockAsync(location);
        
        // Assert
        @lock1.Should().NotBeNull();
        @lock1.GetPrimaryKeyString().Should().Be(location.Value);
        @lock2.Should().NotBeNull();
        @lock2.GetPrimaryKeyString().Should().Be(location.Value);
    }
    
    [Test]
    public async Task When_DeleteLockAsync_LockExists_LockRemoved()
    {
        // Arrange
        var key = Guid.NewGuid().ToString("N");
        var location = new LocationModel(key);
        var buildingKey = Guid.NewGuid().ToString("N");
        var grain = this.GrainFactory.GetGrain<IBuildingGrain>(buildingKey);
        await grain.CreateLockAsync(location);
        
        // Act
        await grain.DeleteLockAsync(location);
        var @lock = await grain.GetLockAsync(location);
        
        // Assert
        @lock.Should().BeNull();
    }
    
}