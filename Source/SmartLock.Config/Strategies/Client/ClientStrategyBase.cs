namespace SmartLock.Config.Strategies.Client;

public abstract class ClientStrategyBase : IClientStrategy
{
    public virtual void Configure(HostApplicationBuilder builder)
    {
        builder.Services.AddOrleansClient(x =>
        {
            ConfigureClient(x, builder.Configuration);
        });
    }

    public virtual void Configure(WebApplicationBuilder builder)
    {
        builder.Services.AddOrleansClient(x =>
        {
            ConfigureClient(x, builder.Configuration);
        });
    }

    public abstract void ConfigureClient(IClientBuilder builder, IConfiguration configuration);
}