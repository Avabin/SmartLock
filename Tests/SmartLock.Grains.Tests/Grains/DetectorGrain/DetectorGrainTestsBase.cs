using FluentAssertions;
using Orleans.TestingHost;
using SmartLock.GrainInterfaces;
using Tests.Shared;

namespace SmartLock.Grains.Tests.Grains.DetectorGrain;

public abstract class DetectorGrainTestsBase<TSiloConfigurator, TClientConfigurator> : GrainTestBase<TSiloConfigurator, TClientConfigurator>
    where TSiloConfigurator : ISiloConfigurator, new()
    where TClientConfigurator : IClientBuilderConfigurator, new()
{
    protected static string CorrectUrl => "https://www.google.com/images/branding/googlelogo/2x/googlelogo_color_272x92dp.png";
    protected static string NotImageUrl => "https://www.google.com/images/branding/googlelogo/2x/internal.docx";
    protected static string BadUrl = ":::::::@:!#:!@:#:!@:#:@! :#:!V:@:#: !:@:#:!:@:#:!@: V#:!:@#V:!@:# :!@: #";
    [Test]
    public async Task When_DetectAsync_DetectionsAreReturned()
    {
        // Arrange
        var key = Guid.NewGuid();
        var grain = this.GrainFactory.GetGrain<IDetectorGrain>(key);
        var imgUrl = CorrectUrl;
        
        // Act
        var result = await grain.DetectAsync(imgUrl);
        
        // Assert
        
        result.Should().NotBeNull();
        foreach (var detections in result)
        {
            foreach (var d in detections.Detections)
            {
                d.Class.Should().NotBeNullOrWhiteSpace();
                d.Confidence.Should().BeGreaterThan(0);
            }
        }
    }

    [Test]
    public async Task When_DetectAsync_UrlNotAnImage_ThrowsArgumentException()
    {
        // Arrange
        var key = Guid.NewGuid();
        var grain = this.GrainFactory.GetGrain<IDetectorGrain>(key);
        var imgUrl = NotImageUrl;
        
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
        var imgUrl = BadUrl;
        
        // Act
        Func<Task> act = async () => await grain.DetectAsync(imgUrl);
        
        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}