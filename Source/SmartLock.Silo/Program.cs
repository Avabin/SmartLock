using SmartLock.Config;
using SmartLock.Config.Strategies;
using Yolov8.Client;

var builder = Host.CreateApplicationBuilder(args);
var yoloUrl = builder.Configuration.GetValue<string>("YoloUrl", "http://127.0.0.1:50051");
builder.Services.AddYoloClient(yoloUrl!);
var strategy = builder.Configuration.GetValue<string>("SiloStrategy",Strategy.Mongo);
builder.UseOrleansStrategy(strategy!);
var app = builder.Build();
app.Run();