using CompreFace.Sdk.Detection;
using CompreFace.Sdk.Recognition;
using FlashCap;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CompreFace.Sdk.Tests;

public class Tests
{
    private IServiceProvider _serviceProvider;

    public Tests()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddCompreFaceSdk(options =>
        {
            options.BaseUrl = "http://localhost:8000";
            options.DetectionApiKey = Guid.Parse("e7e92f32-e29e-4969-982e-5bd36deed68c");
            options.RecognitionApiKey = Guid.Parse("1cf57cf5-826f-442d-812f-92deeef36d7d");
            options.VerificationApiKey = Guid.Parse("8962b7db-e155-461a-bda6-8abd10eb7ae0");
        });
        var host = builder.Build();
        _serviceProvider = host.Services;
    }
    [Test]
    public async Task Test_GetSubjects()
    {
        // Arrange
        var client = _serviceProvider.GetRequiredService<IRecognitionClient>();
        
        // Act
        var result = await client.GetSubjectsAsync();
        
        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }
    
    [Test]
    public async Task AddSubject()
    {
        // Arrange
        var client = _serviceProvider.GetRequiredService<IRecognitionClient>();
        var filePath = @"C:\Users\avabi\Pictures\Zrzut ekranu 2022-12-24 133313.png";
        var image = () => File.OpenRead(filePath);
        var expectedSubject = "Kamil";
        
        // Act
        var result = await client.AddExampleAsync(image, expectedSubject);
        
        // Assert
        result.Should().NotBeNull();
        result.Subject.Should().Be(expectedSubject);
    }

    [Test]
    public async Task Recognize()
    {
        // Arrange
        var client = _serviceProvider.GetRequiredService<IRecognitionClient>();
        var filePath = @"C:\Users\avabi\Pictures\Zrzut ekranu 2022-12-24 133313.png";
        var image = () => File.OpenRead(filePath);
        
        // Act
        var result = await client.RecognizeAsync(image);
        
        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Detection()
    {
        // Arrange
        var client = _serviceProvider.GetRequiredService<IDetectionClient>();
        var filePath = @"C:\Users\avabi\Pictures\Zrzut ekranu 2022-12-24 133313.png";
        var image = () => File.OpenRead(filePath);
        
        // Act
        var result = await client.DetectAsync(image, PixelFormats.PNG);
        
        // Assert
        result.Should().NotBeNull();
    } 
}