using System.Reflection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace SmartLock.Config.Strategies.Silo;

public abstract class SiloStrategyBase : ISiloStrategy
{

    public virtual void Configure(WebApplicationBuilder builder)
    {
        builder.Services.AddOrleans(x =>
        {
            ConfigureCommons(x, builder.Configuration);
            ConfigureSilo(x, builder.Configuration);
        });
    }
    
    public virtual void Configure(HostApplicationBuilder builder)
    {
        builder.Services.AddOrleans(x =>
        {
            ConfigureCommons(x, builder.Configuration);
            ConfigureSilo(x, builder.Configuration);
        });
    }

    private void ConfigureCommons(ISiloBuilder builder, IConfiguration configuration)
    {
        var serviceName = configuration.GetValue<string>("Orleans:ServiceId") ?? "WebApi";
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0";
        var jaegarConnectionString = configuration.GetConnectionString("Jaeger") ?? "localhost:6831";
        var split = jaegarConnectionString.Split(":");
        var jaegarHost = split[0];
        var jaegarPort = int.Parse(split[1]);
        
        builder.AddActivityPropagation();
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics
                .AddMeter("Microsoft.Orleans").AddMeter("Microsoft.AspNetCore.Http.Connections").AddAspNetCoreInstrumentation())
            .WithTracing(tracing =>
            {
                tracing.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(serviceName: serviceName, serviceVersion: version));

                tracing.AddAspNetCoreInstrumentation();
                tracing.AddSource("Microsoft.Orleans.Runtime");
                tracing.AddSource("Microsoft.Orleans.Application");
                tracing.AddSource("SmartLock");
                
                tracing.AddJaegerExporter(options =>
                {
                    options.AgentHost = jaegarHost;
                    options.AgentPort = jaegarPort;
                });
            });
    }

    public abstract void ConfigureSilo(ISiloBuilder builder, IConfiguration configuration);
}