using SmartLock.Orleans.Core;

namespace SmartLock.Config.Strategies.Silo;

public class LocalSiloStrategy : SiloStrategyBase
{
    public override void ConfigureSilo(ISiloBuilder builder, IConfiguration configuration)
    {
        builder.UseLocalhostClustering();
        builder.AddMemoryGrainStorageAsDefault();
        builder.AddMemoryGrainStorage("PubSubStore");
        builder.UseInMemoryReminderService();
        builder.AddMemoryStreams(StreamProviderConstants.DefaultStreamProviderName, configurator =>
        {
                
        }).AddLogStorageBasedLogConsistencyProvider();
    }
}