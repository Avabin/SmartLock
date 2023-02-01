using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;
using SmartLock.Client.Models;
using SmartLock.GrainInterfaces;

namespace SmartLock.WebApi.Controllers;

[ApiController, Route("api/[controller]")]
public class BuildingsController : ControllerBase
{
    [HttpGet("locks")]
    public async Task<IReadOnlyList<LockModel>> GetLocksAsync([FromServices] IClusterClient client, [FromQuery] string building)
    {
        var buildingGrain = client.GetGrain<IJournaledBuildingGrain>(building);
        var locks = await buildingGrain.GetLocksAsync();
        return locks;
    }
    
    [HttpGet("lock/status")]
    public async Task<IActionResult> GetLockStatusAsync([FromServices] IClusterClient client, [FromQuery] string building, [FromQuery] string lockLocation)
    {
        var buildingGrain = client.GetGrain<IJournaledBuildingGrain>(building);
        var lockModel = await buildingGrain.GetLockAsync(new LocationModel(lockLocation));
        if (!lockModel.HasValue)
        {
            return NotFound();
        }

        return Ok(lockModel.Value.IsOpen);
    }
    [HttpPost("locks/status")]
    public async Task<IReadOnlyList<LockModel>> GetLocksStatusAsync([FromServices] IClusterClient client,[FromQuery] string building, [FromBody] IEnumerable<string> locks)
    {
        var locksList = locks.ToList();
        var grain = client.GetGrain<IJournaledBuildingGrain>(building);
        var locksModels = await grain.GetLocksAsync(locksList.Select(x => (LocationModel) x).ToArray());
        return locksModels;
    }
    
    [HttpPost("openLock")]
    public async Task<IActionResult> OpenLockAsync([FromServices] IClusterClient client, [FromQuery] string building, [FromQuery] string lockLocation)
    {
        var buildingGrain = client.GetGrain<IJournaledBuildingGrain>(building);
        var lockModel = await buildingGrain.GetLockAsync(new LocationModel(lockLocation));
        if (lockModel is null)
        {
            return NotFound("Lock not found");
        }
        await buildingGrain.UnlockAsync(lockModel.Value.Location);
        return Ok();
    }
    
    [HttpPost("closeLock")]
    public async Task<IActionResult> CloseLockAsync([FromServices] IClusterClient client, [FromQuery] string building, [FromQuery] string lockLocation)
    {
        var buildingGrain = client.GetGrain<IJournaledBuildingGrain>(building);
        var lockModel = await buildingGrain.GetLockAsync(new LocationModel(lockLocation));
        if (lockModel is null)
        {
            return NotFound("Lock not found");
        }
        await buildingGrain.LockAsync(lockModel.Value.Location);
        return Ok();
    }
    
    [HttpPost("addLock")]
    public async Task<IActionResult> AddLockAsync([FromServices] IClusterClient client, [FromBody] CreateLockModel model)
    {
        var buildingGrain = client.GetGrain<IJournaledBuildingGrain>(model.Building);
        var lockModel = await buildingGrain.GetLockAsync(model.LockLocation);
        if (lockModel is not null)
        {
            return Conflict("Lock already exists");
        }
        await buildingGrain.AddLockAsync(model.LockLocation);
        return Ok();
    }
    
    [HttpDelete("removeLock")]
    public async Task<IActionResult> RemoveLockAsync([FromServices] IClusterClient client, [FromQuery] string building, [FromQuery] string lockLocation)
    {
        var buildingGrain = client.GetGrain<IJournaledBuildingGrain>(building);
        await buildingGrain.RemoveLockAsync(lockLocation);
        return Ok();
    }
    
    [HttpPost("openAll")]
    public async Task<IActionResult> OpenAllAsync([FromServices] IClusterClient client, [FromQuery] string building)
    {
        var buildingGrain = client.GetGrain<IJournaledBuildingGrain>(building);
        await buildingGrain.UnlockAllAsync();
        return Ok();
    }
    
    [HttpPost("closeAll")]
    public async Task<IActionResult> CloseAllAsync([FromServices] IClusterClient client, [FromQuery] string building)
    {
        var buildingGrain = client.GetGrain<IJournaledBuildingGrain>(building);
        await buildingGrain.LockAllAsync();
        return Ok();
    }
    
}

public readonly record struct CreateLockModel(string Building, string LockLocation);