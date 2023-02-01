using Orleans.TestingHost;
using SmartLock.Orleans.Core;

namespace Tests.Shared.SiloConfigurators;

public class UnitTestSiloBuilderConfigurator : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.AddMemoryGrainStorageAsDefault();
        siloBuilder.AddMemoryGrainStorage("PubSubStore");
        siloBuilder.UseInMemoryReminderService();
        siloBuilder.AddLogStorageBasedLogConsistencyProviderAsDefault();
        siloBuilder.AddMemoryStreams(StreamProviderConstants.DefaultStreamProviderName, options =>
        {
            
        });
    }
}