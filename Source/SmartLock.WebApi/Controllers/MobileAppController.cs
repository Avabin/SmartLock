using Microsoft.AspNetCore.Mvc;
using SmartLock.Client.Models;
using SmartLock.GrainInterfaces;

namespace SmartLock.WebApi.Controllers;

[ApiController, Route("api/[controller]")]
public class MobileAppController : ControllerBase
{
    private static string Location = "Building 1";
    private static string FirstLockLocation = "Gate 1";
    private static string SecondLockLocation = "Gate 2";
    private static string ThirdLockLocation = "Gate 3";
    private static string FourthLockLocation = "Gate 4";
    
    [HttpGet]
    public async Task TriggerNotificationAsync([FromServices] IClusterClient client)
    {
        var grain = client.GetGrain<IJournaledBuildingGrain>(Location);

        await grain.AddLockAsync(FirstLockLocation);
        await grain.AddLockAsync(SecondLockLocation);
        await grain.AddLockAsync(ThirdLockLocation);
        await grain.AddLockAsync(FourthLockLocation);

        // open gates sequentially with 1 second delay
        for (int i = 0; i < 5; i++)
        {
            await grain.LockAsync(FirstLockLocation);
            await Task.Delay(1000);
            await grain.UnlockAsync(FirstLockLocation);
            await Task.Delay(1000);
            await grain.LockAsync(SecondLockLocation);
            await Task.Delay(1000);
            await grain.UnlockAsync(SecondLockLocation);
            await Task.Delay(1000);
            await grain.LockAsync(ThirdLockLocation);
            await Task.Delay(1000);
            await grain.UnlockAsync(ThirdLockLocation);
            await Task.Delay(1000);
            await grain.LockAsync(FourthLockLocation);
            await Task.Delay(1000);
            await grain.UnlockAsync(FourthLockLocation);
        }
    }
}