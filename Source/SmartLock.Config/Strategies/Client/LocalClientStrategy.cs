using SmartLock.Orleans.Core;

namespace SmartLock.Config.Strategies.Client;

public class LocalClientStrategy : ClientStrategyBase
{
    public override void ConfigureClient(IClientBuilder builder, IConfiguration configuration)
    {
        builder.UseLocalhostClustering().AddMemoryStreams(StreamProviderConstants.DefaultStreamProviderName);
    }
}