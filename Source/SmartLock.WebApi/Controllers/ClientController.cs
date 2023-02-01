using Microsoft.AspNetCore.Mvc;
using SmartLock.Client.Models;
using SmartLock.GrainInterfaces;

namespace SmartLock.WebApi.Controllers;

[ApiController, Route("api/[controller]")]
public class ClientController : ControllerBase
{
    [HttpGet("settings")]
    public async Task<ClientSettings> GetSettingsAsync([FromServices] IClusterClient client, [FromQuery] string deviceId)
    {
        return await client.GetGrain<IClientGrain>(deviceId).GetSettingsAsync();
    }
    
    [HttpPost("register")]
    public async Task RegisterAsync([FromServices] IClusterClient client, [FromBody] RegisterClientModel model)
    {
        await client.GetGrain<IClientGrain>(model.DeviceId).RegisterAsync(model);
    }
    
    
    
}