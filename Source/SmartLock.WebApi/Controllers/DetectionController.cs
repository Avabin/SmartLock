using System.Buffers;
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
        var stream = file.OpenReadStream();
        var rent = MemoryPool<byte>.Shared.Rent((int)file.Length);
        _ = await stream.ReadAsync(rent.Memory, CancellationToken.None);
        var detectionResults = await client.GetGrain<IDetectorGrain>(Guid.NewGuid()).DetectAsync(new DetectionRequest(rent.Memory.ToArray().ToImmutableArray()));
        return detectionResults.SelectMany(x => x.Detections).ToList();
    }

}