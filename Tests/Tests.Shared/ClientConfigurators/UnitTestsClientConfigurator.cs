using Microsoft.Extensions.Configuration;
using Orleans.TestingHost;
using SmartLock.Config.Strategies.Client;
using SmartLock.Orleans.Core;

namespace Tests.Shared.ClientConfigurators;

public class UnitTestsClientConfigurator : IClientBuilderConfigurator
{
    public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
    {
        clientBuilder.AddMemoryStreams(StreamProviderConstants.DefaultStreamProviderName);
    }
}