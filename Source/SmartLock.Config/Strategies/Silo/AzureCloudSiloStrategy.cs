using System.Net;
using SmartLock.Orleans.Core;

namespace SmartLock.Config.Strategies.Silo;

public class AzureTableSiloStrategy : SiloStrategyBase
{
    public override void ConfigureSilo(ISiloBuilder builder, IConfiguration configuration)
    {
        var azureConnectionString = configuration.GetConnectionString("AzureStorage") ??
                                    throw new InvalidOperationException("AzureStorage connection string is missing");
        var siloPort = configuration.GetValue<int?>("Orleans:SiloPort") ?? 11111;
        builder.ConfigureEndpoints(IPAddress.Any, siloPort, 30000);
        builder.UseAzureStorageClustering(options =>
            {
                options.ConfigureTableServiceClient(azureConnectionString);
            })
            .AddAzureTableGrainStorageAsDefault(options =>
            {
                options.ConfigureTableServiceClient(azureConnectionString);
            })
            .UseAzureTableReminderService(azureConnectionString)
            .AddAzureTableGrainStorage("PubSubStore", options =>
            {
                options.ConfigureTableServiceClient(azureConnectionString);

            }).AddAzureQueueStreams(StreamProviderConstants.DefaultStreamProviderName, optionsBuilder =>
            {
                optionsBuilder.Configure(x => x.ConfigureQueueServiceClient(azureConnectionString));
            }).AddLogStorageBasedLogConsistencyProvider();
    }
}