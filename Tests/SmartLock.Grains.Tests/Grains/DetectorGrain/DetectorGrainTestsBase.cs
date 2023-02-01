using FluentAssertions;
using Orleans.TestingHost;
using SmartLock.GrainInterfaces;
using Tests.Shared;

namespace SmartLock.Grains.Tests.DetectorGrain;

public abstract class DetectorGrainTestsBase<TSiloConfigurator, TClientConfigurator> : GrainTestBase<TSiloConfigurator, TClientConfigurator>
    where TSiloConfigurator : ISiloConfigurator, new()
    where TClientConfigurator : IClientBuilderConfigurator, new()
{
    [Test]
    public async Task When_DetectAsync_DetectionsAreReturned()
    {
        // Arrange
        var key = Guid.NewGuid();
        var grain = this.GrainFactory.GetGrain<IDetectorGrain>(key);
        var imgUrl = "https://www.google.com/images/branding/googlelogo/2x/googlelogo_color_272x92dp.png";
        
        // Act
        var result = await grain.DetectAsync(imgUrl);
        
        // Assert
        
        result.Should().NotBeNull();
        result.Detections.Should().NotBeEmpty();
    }

    [Test]
    public async Task When_DetectAsync_UrlNotAnImage_ThrowsArgumentException()
    {
        // Arrange
        var key = Guid.NewGuid();
        var grain = this.GrainFactory.GetGrain<IDetectorGrain>(key);
        var imgUrl = "https://www.google.com/images/branding/googlelogo/2x/internal.docx";
        
        // Act
        Func<Task> act = async () => await grain.DetectAsync(imgUrl);
        
        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
    
    [Test]
    public async Task When_DetectAsync_BadUrl_ThrowsInvalidOperationException()
    {
        // Arrange
        var key = Guid.NewGuid();
        var grain = this.GrainFactory.GetGrain<IDetectorGrain>(key);
        var imgUrl = ":::::::@:!#:!@:#:!@:#:@! :#:!V:@:#: !:@:#:!:@:#:!@: V#:!:@#V:!@:# :!@: #";
        
        // Act
        Func<Task> act = async () => await grain.DetectAsync(imgUrl);
        
        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}