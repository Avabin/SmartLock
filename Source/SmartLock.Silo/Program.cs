using SmartLock.Config;
using SmartLock.Config.Strategies;

var builder = WebApplication.CreateBuilder(args);
var strategy = builder.Configuration.GetValue<string>("SiloStrategy",Strategy.Mongo);
builder.UseOrleansStrategy(strategy!);
var app = builder.Build();
app.Run();