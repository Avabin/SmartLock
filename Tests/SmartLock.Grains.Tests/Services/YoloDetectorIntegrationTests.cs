using FluentAssertions;
using SmartLock.Client.HttpClient;
using Tests.Shared;
using Yolov8.Client;

namespace SmartLock.Grains.Tests.Services;

[TestFixture, Category("Yolo"), Category("Integration"), Parallelizable(ParallelScope.Fixtures)]
public class YoloDetectorIntegrationTests : UnitTestsBase
{
    protected override void ConfigureServices(HostBuilderContext environment, IServiceCollection services)
    {
        services.AddYoloClient("http://127.0.0.1:50051");
    }

    protected override void Configure(HostBuilderContext environment, IConfigurationBuilder builder)
    {
        
    }

    [Test]
    public async Task DetectUrlTestAsync()
    {
        // Arrange
        var imageUrl = "https://www.akc.org/wp-content/uploads/2017/11/Chinook-On-White-03.jpg";
        var client = this.Services.GetRequiredService<IYoloClient>();
        
        // Act
        var detections = await client.DetectAsync(imageUrl).ConfigureAwait(false);
        // Assert
        detections.Should().NotBeNull();
    }
    
    [Test]
    public async Task DetectStreamTestAsync()
    {
        // Arrange
        var imageUrl = "https://www.akc.org/wp-content/uploads/2017/11/Chinook-On-White-03.jpg";
        var client = this.Services.GetRequiredService<IYoloClient>();
        var imgClient = new HttpClient();
        var image = await imgClient.GetStreamAsync(imageUrl).ConfigureAwait(false);
        
        // Act
        var detections = await client.DetectAsync(image).ConfigureAwait(false);
        // Assert
        detections.Should().NotBeNull();
    }
}