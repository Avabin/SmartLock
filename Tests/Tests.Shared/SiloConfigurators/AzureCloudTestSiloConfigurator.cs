using Orleans.TestingHost;
using SmartLock.Orleans.Core;
using Yolov8.Client;

namespace Tests.Shared.SiloConfigurators;

public class AzureCloudTestSiloConfigurator : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        var azureConnectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING") ??
                                    "UseDevelopmentStorage=true";
        var yoloBaseUrl = Environment.GetEnvironmentVariable("YOLO_BASE_URL") ?? "http://localhost:50051";
        siloBuilder.Services.AddYoloClient(yoloBaseUrl);
        siloBuilder
            .UseAzureTableReminderService(options => options.ConfigureTableServiceClient(azureConnectionString))
            .UseAzureStorageClustering(options => options.ConfigureTableServiceClient(azureConnectionString))
            .AddAzureTableGrainStorageAsDefault(options => options.ConfigureTableServiceClient(azureConnectionString))
            .AddAzureTableGrainStorage("PubSubStore",
                options => options.ConfigureTableServiceClient(azureConnectionString))
            .AddAzureQueueStreams(StreamProviderConstants.DefaultStreamProviderName,
                options => options.Configure(x => x.ConfigureQueueServiceClient(azureConnectionString)))
            .AddLogStorageBasedLogConsistencyProviderAsDefault();
    }
}