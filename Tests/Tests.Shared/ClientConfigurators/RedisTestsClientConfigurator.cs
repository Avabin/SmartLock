using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Orleans.TestingHost;
using SmartLock.Config.Strategies.Client;
using SmartLock.Orleans.Core;
using SmartLock.Streams.RabbitMQ.Configurators;

namespace Tests.Shared.ClientConfigurators;

public class RedisTestsClientConfigurator : IClientBuilderConfigurator
{
    public static IConfigurationBuilder ConfigurationBuilder { get; } = new ConfigurationBuilder()
        .AddJsonFile("appsettings.Development.json")
        .AddEnvironmentVariables();
    public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
    {
        var configurationRoot = ConfigurationBuilder.Build();
        clientBuilder.AddRabbitMQStreams(StreamProviderConstants.DefaultStreamProviderName, configurationRoot.GetRequiredSection("RabbitMQ"));
    }
}