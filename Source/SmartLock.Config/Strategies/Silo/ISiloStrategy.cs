namespace SmartLock.Config.Strategies.Silo;

public interface ISiloStrategy
{
    void Configure(WebApplicationBuilder builder);
    void Configure(HostApplicationBuilder builder);
}