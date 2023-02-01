using System.Runtime.CompilerServices;

using Orleans.Runtime;
using Orleans.Streams;
using SmartLock.Client;
using SmartLock.Client.Models;
using SmartLock.Client.NotificationHub;
using SmartLock.Config;
using SmartLock.Config.Strategies;
using SmartLock.Orleans.Core;
using SmartLock.WebApi.Hubs;

[assembly: InternalsVisibleTo("SmartLock.Grains.Tests")]
var builder = WebApplication.CreateBuilder(args);

var strategy = builder.Configuration.GetValue<string>("ClientStrategy", Strategy.Local);
builder.UseOrleansClientStrategy(strategy!);

// Add services to the container.
builder.Services.AddSingleton<NotificationsHubService>();
builder.Services.AddSignalR();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationsHub>(INotificationsHubClient.HubPath);

await app.StartAsync();

var clusterClient = app.Services.GetRequiredService<IClusterClient>();
var streamProvider = clusterClient.GetStreamProvider(StreamProviderConstants.DefaultStreamProviderName);
var stream = streamProvider.GetStream<Notification<IEvent>>(StreamId.Create(StreamProviderConstants.NotificationsNamespace, "Building 1"));
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var handle = await stream.SubscribeAsync(async (notification, token) =>
{
    logger.LogInformation("Received notification: {@Notification}", notification);
});

await app.WaitForShutdownAsync();

await handle.UnsubscribeAsync();
public partial class Program { }
