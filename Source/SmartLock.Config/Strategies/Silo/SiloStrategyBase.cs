namespace SmartLock.Config.Strategies.Silo;

/// <summary>
/// Uses localhost cluster configuration
/// and in memory storage
/// </summary>
public abstract class SiloStrategyBase : ISiloStrategy
{

    public virtual void Configure(WebApplicationBuilder builder)
    {
        builder.Services.AddOrleans(x =>
        {
            ConfigureSilo(x, builder.Configuration);
        });
    }
    
    public virtual void Configure(HostApplicationBuilder builder)
    {
        builder.Services.AddOrleans(x =>
        {
            ConfigureSilo(x, builder.Configuration);
        });
    }
    
    public abstract void ConfigureSilo(ISiloBuilder builder, IConfiguration configuration);
}