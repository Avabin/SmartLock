using FluentAssertions;
using Orleans.TestingHost;
using SmartLock.Client;
using SmartLock.Client.HttpClient;
using SmartLock.Client.Models;
using SmartLock.GrainInterfaces;
using Tests.Shared;

namespace SmartLock.Grains.Tests.WebApi;

public abstract class SmartLockWebApiTestsBase<TSilo,TClient, TEntrypoint> : WebApiTestsBase<TSilo,TClient, TEntrypoint> where TSilo : ISiloConfigurator, new() where TEntrypoint : class, IHasClusterClient where TClient : IClientBuilderConfigurator, new()
{
    [Test]
    public async Task When_ClientController_GetSettingsAsync_Unregistered_Then_ReturnsEmptySettings()
    {
        // Arrange
        var deviceId = Random.Shared.Next().ToString();
        var expected = new ClientSettings
        {
            DefaultBuilding = "",
            Name = "",
            DeviceId = ""
        };
        var client = Client;
        
        // Act
        var actual = await client.GetClientSettingsAsync(deviceId);
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    // api/Client/register
    [Test]
    public async Task When_ClientController_RegisterAsync_Then_GetSettingAsync_ReturnsSettings()
    {
        // Arrange
        var deviceId = Random.Shared.Next().ToString();
        var expectedBuilding = "Building 1";
        var expectedName = "Test Client";
        var expected = new ClientSettings
        {
            DefaultBuilding = new LocationModel(expectedBuilding),
            Name = expectedName,
            DeviceId = deviceId
        };
        var client = Client;
        
        // Act
        await client.RegisterClientAsync(deviceId, expectedName, expectedBuilding);
        var actual = await client.GetClientSettingsAsync(deviceId);
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    // api/Buildings/locks
    [Test]
    public async Task When_BuildingController_GetLocksAsync_Then_ReturnsLocks()
    {
        // Arrange
        var randstr = Random.Shared.Next().ToString();
        var building = $"Building 1|{randstr}";
        var lock1 = new LockModel(new LocationModel($"Lock 1|{randstr}"), false);
        var lock2 = new LockModel(new LocationModel($"Lock 2|{randstr}"), false);
        var expected = new[] {lock1, lock2}.ToList();

        var grain = Cluster!.GrainFactory.GetGrain<IJournaledBuildingGrain>(building);
        await grain.AddLockAsync(lock1.Location);
        await grain.AddLockAsync(lock2.Location);

        var client = Client;
        
        // Act
        var actual = await client.GetLocksAsync(building);
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    // api/Buildings/lock/status
    [Test]
    public async Task When_BuildingController_GetLockStatusAsync_Then_ReturnsLockStatus()
    {
        // Arrange
        var randstr = Random.Shared.Next().ToString();
        var building = $"Building 1|{randstr}";
        var expected = new LockModel($"Lock 1|{randstr}", false);
        
        var grain = Cluster!.GrainFactory.GetGrain<IJournaledBuildingGrain>(building);
        await grain.AddLockAsync(expected.Location);

        var client = Client;
        
        // Act
        var actual = await client.GetLockStatusAsync(building, expected.Location);
        
        // Assert
        actual.Should().Be(expected.IsOpen);
    }
    
    // api/Buildings/locks/status
    [Test]
    public async Task When_BuildingController_GetLocksStatusAsync_Then_ReturnsLocksStatus()
    {
        // Arrange
        var randstr = Random.Shared.Next().ToString();
        var building = $"Building 1|{randstr}";
        var lock1 = new LockModel($"Lock 1|{randstr}", false);
        var lock2 = new LockModel($"Lock 2|{randstr}", false);
        var expected = new[] {lock1, lock2}.ToList();

        var grain = Cluster!.GrainFactory.GetGrain<IJournaledBuildingGrain>(building);
        await grain.AddLockAsync(lock1.Location);
        await grain.AddLockAsync(lock2.Location);

        var client = Client;
        
        // Act
        var actual = await client.GetLocksStatusAsync(building, lock1.Location, lock2.Location);
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    // api/Buildings/openLock
    [Test]
    public async Task When_BuildingController_OpenLockAsync_Then_ReturnsLockStatus()
    {
        // Arrange
        var randstr = Random.Shared.Next().ToString();
        var building = $"Building 1|{randstr}";
        var expected = new LockModel($"Lock 1|{randstr}", true);
        
        var grain = Cluster!.GrainFactory.GetGrain<IJournaledBuildingGrain>(building);
        await grain.AddLockAsync(expected.Location);

        var client = Client;
        
        // Act
        await client.OpenLockAsync(building, expected.Location);
        var actual = await client.GetLockStatusAsync(building, expected.Location);
        
        // Assert
        actual.Should().Be(expected.IsOpen);
    }
    
    // api/Buildings/closeLock
    [Test]
    public async Task When_BuildingController_CloseLockAsync_Then_ReturnsLockStatus()
    {
        // Arrange
        var randstr = Random.Shared.Next().ToString();
        var building = $"Building 1|{randstr}";
        var expected = new LockModel($"Lock 1|{randstr}", false);
        
        var grain = Cluster!.GrainFactory.GetGrain<IJournaledBuildingGrain>(building);
        await grain.AddLockAsync(expected.Location);

        var client = Client;
        
        // Act
        await client.CloseLockAsync(building, expected.Location);
        var actual = await client.GetLockStatusAsync(building, expected.Location);
        
        // Assert
        actual.Should().Be(expected.IsOpen);
    }
    
    // api/Buildings/addLock
    [Test]
    public async Task When_BuildingController_AddLockAsync_Then_ReturnsLocks()
    {
        // Arrange
        var randstr = Random.Shared.Next().ToString();
        var building = $"Building 1|{randstr}";
        var lock1 = new LockModel($"Lock 1|{randstr}", false);
        var lock2 = new LockModel($"Lock 2|{randstr}", false);
        var expected = new[] {lock1, lock2}.ToList();

        var client = Client;
        
        // Act
        await client.AddLockAsync(building, lock1.Location);
        await client.AddLockAsync(building, lock2.Location);
        var actual = await client.GetLocksAsync(building);
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    // api/Buildings/removeLock
    [Test]
    public async Task When_BuildingController_RemoveLockAsync_Then_ReturnsLocks()
    {
        // Arrange
        var randstr = Random.Shared.Next().ToString();
        var building = $"Building 1|{randstr}";
        var lock1 = new LockModel($"Lock 1|{randstr}", false);
        var lock2 = new LockModel($"Lock 2|{randstr}", false);
        var expected = new[] {lock1}.ToList();

        var client = Client;
        
        // Act
        await client.AddLockAsync(building, lock1.Location);
        await client.AddLockAsync(building, lock2.Location);
        await client.RemoveLockAsync(building, lock2.Location);
        var actual = await client.GetLocksAsync(building);
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    // api/Buildings/openAll
    [Test]
    public async Task When_BuildingController_OpenAllLocksAsync_Then_ReturnsLocksStatus()
    {
        // Arrange
        var randstr = Random.Shared.Next().ToString();
        var building = $"Building 1|{randstr}";
        var lock1 = new LockModel($"Lock 1|{randstr}", true);
        var lock2 = new LockModel($"Lock 2|{randstr}", true);
        var expected = new[] {lock1, lock2}.ToList();

        var grain = Cluster!.GrainFactory.GetGrain<IJournaledBuildingGrain>(building);
        await grain.AddLockAsync(lock1.Location);
        await grain.AddLockAsync(lock2.Location);

        var client = Client;
        
        // Act
        await client.OpenAllLocksAsync(building);
        var actual = await client.GetLocksStatusAsync(building, lock1.Location, lock2.Location);
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    // api/Buildings/closeAll
    [Test]
    public async Task When_BuildingController_CloseAllLocksAsync_Then_ReturnsLocksStatus()
    {
        // Arrange
        var randstr = Random.Shared.Next().ToString();
        var building = $"Building 1|{randstr}";
        var lock1 = new LockModel($"Lock 1|{randstr}", false);
        var lock2 = new LockModel($"Lock 2|{randstr}", false);
        var expected = new[] {lock1, lock2}.ToList();

        var grain = Cluster!.GrainFactory.GetGrain<IJournaledBuildingGrain>(building);
        await grain.AddLockAsync(lock1.Location);
        await grain.AddLockAsync(lock2.Location);

        var client = Client;
        
        // Act
        await client.CloseAllLocksAsync(building);
        var actual = await client.GetLocksStatusAsync(building, lock1.Location, lock2.Location);
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}