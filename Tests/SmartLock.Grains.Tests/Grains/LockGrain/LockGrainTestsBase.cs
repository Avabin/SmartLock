using FluentAssertions;
using Orleans.TestingHost;
using SmartLock.Client.Models;
using SmartLock.GrainInterfaces;
using Tests.Shared;

namespace SmartLock.Grains.Tests.LockGrain;

public class LockGrainTestsBase<TSiloConfigurator, TClientConfigurator> : GrainTestBase<TSiloConfigurator, TClientConfigurator>
    where TSiloConfigurator : ISiloConfigurator, new()
    where TClientConfigurator : IClientBuilderConfigurator, new()
{
    [Test]
    public async Task When_LockAsync_LockIsLocked()
    {
        // Arrange
        var key = Guid.NewGuid().ToString("N");
        var location = new LocationModel(key);
        var grain = this.GrainFactory.GetGrain<ILockGrain>(location.Value);
        
        // Act
        await grain.LockAsync();
        var isLocked = await grain.IsLockedAsync();
        
        //
        isLocked.Should().BeTrue();
    }
    
    [Test]
    public async Task When_UnlockAsync_LockIsUnlocked()
    {
        // Arrange
        var key = Guid.NewGuid().ToString("N");
        var location = new LocationModel(key);
        var grain = this.GrainFactory.GetGrain<ILockGrain>(location.Value);
        
        // Act
        await grain.UnlockAsync();
        var isLocked = await grain.IsLockedAsync();
        
        //
        isLocked.Should().BeFalse();
    }
}