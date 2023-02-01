namespace SmartLock.Config.Strategies.Client;

public interface IClientStrategy
{
    void Configure(HostApplicationBuilder builder);
    void Configure(WebApplicationBuilder builder);
}