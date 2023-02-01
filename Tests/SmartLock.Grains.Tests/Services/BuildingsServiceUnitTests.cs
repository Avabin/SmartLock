using System.Net;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using SmartLock.Client.HttpClient;
using SmartLock.Client.Models;
using Tests.Shared;

namespace SmartLock.Grains.Tests.Services;

[TestFixture, Category("Unit"), FixtureLifeCycle(LifeCycle.SingleInstance), Parallelizable(ParallelScope.Fixtures)]
public class BuildingsServiceUnitTests : UnitTestsBase
{
    private Mock<HttpMessageHandler> _handler;


    protected override void ConfigureServices(HostBuilderContext environment, IServiceCollection services)
    {
        _handler = new Mock<HttpMessageHandler>();
        var client = new HttpClient(_handler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var factory = Mock.Of<IHttpClientFactory>(f => f.CreateClient(HttpClientConstants.Name) == client);
        services.AddSingleton(factory);

        services.AddTransient<IBuildingsService, BuildingsService>();
    }

    protected override void Configure(HostBuilderContext environment, IConfigurationBuilder builder)
    {
        
    }
    
    // GetLocksAsync
    [Test]
    public async Task When_GetLocksAsync_Then_ReturnsLocks()
    {
        // Arrange
        var randstr = Guid.NewGuid().ToString();
        var building = new LocationModel($"Building-{randstr}");
        var expected = new[]
        {
            new LockModel(new LocationModel("Lock 1"), false),
            new LockModel(new LocationModel("Lock 2"), false)
        };
        
        On(HttpMethod.Get, $"api/Buildings/locks", new StringContent(JsonConvert.SerializeObject(expected)));
        // Act
        var actual = await Services.GetRequiredService<IBuildingsService>().GetLocksAsync(building);
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    // GetLockAsync
    [Test]
    public async Task When_GetLockAsync_Then_ReturnsLock()
    {
        // Arrange
        var randstr = Guid.NewGuid().ToString();
        var building = new LocationModel($"Building-{randstr}");
        var lockLocation = new LocationModel("Lock");
        var expected = false;
        
        On(HttpMethod.Get, $"api/Buildings/lock", new StringContent(JsonConvert.SerializeObject(expected)));
        // Act
        var actual = await Services.GetRequiredService<IBuildingsService>().GetLockAsync(lockLocation, building);
        
        // Assert
        actual.Should().BeEquivalentTo(new LockModel(lockLocation, expected));
    }
    
    // GetLocksAsync(params)
    [Test]
    public async Task When_GetLocksAsync_Params_Then_ReturnsLocks()
    {
        // Arrange
        var randstr = Guid.NewGuid().ToString();
        var building = new LocationModel($"Building-{randstr}");
        var lock1 = new LocationModel("Lock 1");
        var lock2 = new LocationModel("Lock 2");
        var expected = new[]
        {
            new LockModel(lock1, false),
            new LockModel(lock2, false)
        };
        
        On(HttpMethod.Post, "api/Buildings/locks", new StringContent(JsonConvert.SerializeObject(expected)));
        
        // Act
        var actual = await Services.GetRequiredService<IBuildingsService>().GetLocksAsync(building, lock1, lock2);
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    // OpenLockAsync
    [Test]
    public async Task When_OpenLockAsync_OpensLock()
    {
        // Arrange
        var randstr = Guid.NewGuid().ToString();
        var building = new LocationModel($"Building-{randstr}");
        var lockLocation = new LocationModel("Lock");
        var expected = true;
        
        On(HttpMethod.Post, "api/Buildings/openLock", new StringContent(JsonConvert.SerializeObject(new { })));
        On(HttpMethod.Get, "api/Buildings/lock", new StringContent(JsonConvert.SerializeObject(expected)));
        
        // Act
        var sut = Services.GetRequiredService<IBuildingsService>();
        await sut.OpenLockAsync(lockLocation, building);
        var actual = await sut.GetLockAsync(lockLocation, building);
        
        // Assert
        actual.IsOpen.Should().Be(expected);
    }
    
    // CloseLockAsync
    [Test]
    public async Task When_CloseLockAsync_ClosesLock()
    {
        // Arrange
        var randstr = Guid.NewGuid().ToString();
        var building = new LocationModel($"Building-{randstr}");
        var lockLocation = new LocationModel("Lock");
        var expected = false;
        
        On(HttpMethod.Post, "api/Buildings/closeLock", new StringContent(JsonConvert.SerializeObject(new { })));
        On(HttpMethod.Get, "api/Buildings/lock/status", new StringContent(JsonConvert.SerializeObject(expected)));
        
        // Act
        var sut = Services.GetRequiredService<IBuildingsService>();
        await sut.CloseLockAsync(lockLocation, building);
        var actual = await sut.GetLockAsync(lockLocation, building);
        
        // Assert
        actual.IsOpen.Should().Be(expected);
    }
    
    // AddLockAsync
    [Test]
    public async Task When_AddLockAsync_AddsLock()
    {
        // Arrange
        var randstr = Guid.NewGuid().ToString();
        var building = new LocationModel($"Building-{randstr}");
        var lockLocation = new LocationModel("Lock");
        var expected = new LockModel(lockLocation, false);
        
        On(HttpMethod.Post, "api/Buildings/addLock", new StringContent(JsonConvert.SerializeObject(new { })));
        On(HttpMethod.Get, "api/Buildings/lock", new StringContent(JsonConvert.SerializeObject(false)));
        
        // Act
        var sut = Services.GetRequiredService<IBuildingsService>();
        await sut.AddLockAsync(lockLocation, building);

        var actual = await sut.GetLockAsync(lockLocation, building);
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    // RemoveLockAsync
    [Test]
    public async Task When_RemoveLockAsync_RemovesLock()
    {
        // Arrange
        var randstr = Guid.NewGuid().ToString();
        var building = new LocationModel($"Building-{randstr}");
        var lockLocation = new LocationModel("Lock");

        On(HttpMethod.Delete, "api/Buildings/removeLock", new StringContent(JsonConvert.SerializeObject(new { })));
        On(HttpMethod.Get, "api/Buildings/lock", new StringContent(JsonConvert.SerializeObject("")), HttpStatusCode.NotFound);
        
        // Act
        var sut = Services.GetRequiredService<IBuildingsService>();
        await sut.RemoveLockAsync(lockLocation, building);
        var act = async () => await sut.GetLockAsync(lockLocation, building);
        
        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }
    
    // OpenAllLocksAsync
    [Test]
    public async Task When_OpenAllLocksAsync_OpensAllLocks()
    {
        // Arrange
        var randstr = Guid.NewGuid().ToString();
        var building = new LocationModel($"Building-{randstr}");
        var lock1 = new LocationModel("Lock 1");
        var lock2 = new LocationModel("Lock 2");
        var expected = new[]
        {
            new LockModel(lock1, true),
            new LockModel(lock2, true)
        };

        On(HttpMethod.Post, "api/Buildings/openAll", new StringContent(JsonConvert.SerializeObject(new { })));
        On(HttpMethod.Post, "api/Buildings/locks", new StringContent(JsonConvert.SerializeObject(expected)));
        
        // Act
        var sut = Services.GetRequiredService<IBuildingsService>();
        await sut.OpenAllLocksAsync(building);
        var actual = await sut.GetLocksAsync(building, lock1, lock2);
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    // CloseAllLocksAsync
    [Test]
    public async Task When_CloseAllLocksAsync_ClosesAllLocks()
    {
        // Arrange
        var randstr = Guid.NewGuid().ToString();
        var building = new LocationModel($"Building-{randstr}");
        var lock1 = new LocationModel("Lock 1");
        var lock2 = new LocationModel("Lock 2");
        var expected = new[]
        {
            new LockModel(lock1, false),
            new LockModel(lock2, false)
        };
        
        On(HttpMethod.Post, "api/Buildings/closeAll", new StringContent(JsonConvert.SerializeObject(new {})));
        On(HttpMethod.Post, "api/Buildings/locks/status", new StringContent(JsonConvert.SerializeObject(expected)));

        // Act
        var sut = Services.GetRequiredService<IBuildingsService>();
        await sut.CloseAllLocksAsync(building);
        var actual = await sut.GetLocksAsync(building, lock1, lock2);
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    private void On(HttpMethod method, string path, HttpContent content, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        _handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(message => message.Method == method && message.RequestUri.AbsoluteUri.Contains(path)), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(statusCode)
            {
                Content = content
            });
    }
}