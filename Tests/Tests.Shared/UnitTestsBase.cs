using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace Tests.Shared;

public abstract class UnitTestsBase
{
    private IHostBuilder HostBuilder => CreateHostBuilder(Array.Empty<string>());
    private IHost TestHost { get; set; } = null!;
    protected IServiceProvider Services => TestHost.Services;
    protected IConfiguration Configuration => TestHost.Services.GetRequiredService<IConfiguration>();
    protected IHostEnvironment Environment => TestHost.Services.GetRequiredService<IHostEnvironment>();

    private IHostBuilder CreateHostBuilder(string[] args) => 
        Host.CreateDefaultBuilder(args)
            .ConfigureServices(ConfigureServices)
            .ConfigureAppConfiguration(Configure);
    
    protected abstract void ConfigureServices(HostBuilderContext environment, IServiceCollection services);
    protected abstract void Configure(HostBuilderContext environment, IConfigurationBuilder builder);

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        TestHost = HostBuilder.Build();
        await TestHost.StartAsync();
    }
    
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await TestHost.StopAsync();
        await TestHost.WaitForShutdownAsync();
    }
}