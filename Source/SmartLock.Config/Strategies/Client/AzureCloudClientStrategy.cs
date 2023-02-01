using SmartLock.Orleans.Core;

namespace SmartLock.Config.Strategies.Client;

public class AzureCloudClientStrategy : ClientStrategyBase
{
    public override void ConfigureClient(IClientBuilder builder, IConfiguration configuration)
    {
        var azureConnectionString = configuration.GetConnectionString("AzureStorage") ?? "UseDevelopmentStorage=true";
        builder.UseAzureStorageClustering(options => options.ConfigureTableServiceClient(azureConnectionString))
            .AddAzureQueueStreams(StreamProviderConstants.DefaultStreamProviderName,
                options => options.Configure(x => x.ConfigureQueueServiceClient(azureConnectionString)));
    }
}