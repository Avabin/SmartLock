using System.ComponentModel.DataAnnotations;

namespace CompreFace.Sdk;

public class CompreFaceSdkOptions
{
    [Url]
    public string BaseUrl { get; set; } = "http://localhost:8000";
    [Required]
    public Guid RecognitionApiKey { get; set; } = Guid.Empty;
    
    [Required]
    public Guid DetectionApiKey { get; set; } = Guid.Empty;
    
    [Required]
    public Guid TrackingApiKey { get; set; } = Guid.Empty;
}