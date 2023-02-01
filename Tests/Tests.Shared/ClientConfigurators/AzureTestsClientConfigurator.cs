using Microsoft.Extensions.Configuration;
using Orleans.TestingHost;
using SmartLock.Config.Strategies.Client;
using SmartLock.Orleans.Core;

namespace Tests.Shared.ClientConfigurators;

public class AzureTestsClientConfigurator : IClientBuilderConfigurator
{
    public static IConfigurationBuilder ConfigurationBuilder { get; } = new ConfigurationBuilder()
        .AddJsonFile("appsettings.Development.json")
        .AddEnvironmentVariables();
    public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
    {
        var azureConnectionString = configuration.GetConnectionString("AzureStorage") ?? "UseDevelopmentStorage=true";
        clientBuilder.AddAzureQueueStreams(StreamProviderConstants.DefaultStreamProviderName,
                options => options.Configure(x => x.ConfigureQueueServiceClient(azureConnectionString)));
    }
}