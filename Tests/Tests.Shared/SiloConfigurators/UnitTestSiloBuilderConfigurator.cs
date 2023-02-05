using Microsoft.Extensions.DependencyInjection;
using Moq;
using Orleans.TestingHost;
using SmartLock.Orleans.Core;
using Yolov8.Client;

namespace Tests.Shared.SiloConfigurators;

public class UnitTestSiloBuilderConfigurator : ISiloConfigurator
{
    public static readonly Mock<IYoloClient> YoloClientMock = new();
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.Services.AddSingleton<IYoloClient>(sp => YoloClientMock.Object);
        siloBuilder.AddMemoryGrainStorageAsDefault();
        siloBuilder.AddMemoryGrainStorage("PubSubStore");
        siloBuilder.UseInMemoryReminderService();
        siloBuilder.AddLogStorageBasedLogConsistencyProviderAsDefault();
        siloBuilder.AddMemoryStreams(StreamProviderConstants.DefaultStreamProviderName, options =>
        {
            
        });
    }
}