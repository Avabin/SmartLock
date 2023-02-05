using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;
using SmartLock.Client.Models;
using SmartLock.GrainInterfaces;
using SmartLock.ObjectStorage;

namespace SmartLock.WebApi.Controllers;

[ApiController, Route("api/[controller]")]
public class DetectionController : ControllerBase
{
    [HttpGet("detect")]
    public async Task<IReadOnlyList<DetectedObjectModel>> DetectAsync([FromServices] IClusterClient client, [FromQuery] string imgUrl)
    {
        var detectionResults = await client.GetGrain<IDetectorGrain>(Guid.NewGuid()).DetectAsync(imgUrl);
        return detectionResults.SelectMany(x => x.Detections).ToList();
    }
    
    [HttpPost("detect")]
    public async Task<IReadOnlyList<DetectedObjectModel>> DetectAsync([FromServices] IClusterClient client, [FromServices] IObjectStorage objectStorage, [FromForm] IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var detectionResults = await client.GetGrain<IDetectorGrain>(Guid.NewGuid()).DetectAsync(new DetectionRequest(ms.ToArray().ToImmutableArray()));
        return detectionResults.SelectMany(x => x.Detections).ToList();
    }

}