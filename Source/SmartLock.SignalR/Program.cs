using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartLock.Client;
using SmartLock.Client.Models;
using SmartLock.Client.NotificationHub;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSmartLockClient("http://localhost:5067");

var app = builder.Build();
await app.StartAsync();

var client = (NotificationsHubClient) app.Services.GetRequiredService<INotificationsHubClient>();
var observable =app.Services.GetRequiredService<IObservable<Notification<IEvent>>>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var subscription = observable.Subscribe(notification =>
{
    logger.LogInformation("Received notification: {@Notification}", notification);
});
await client.StartAsync();
await client.SubscribeAsync("Building 1");

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

await app.StopAsync();
subscription.Dispose();