using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.LockGrain;

[Category("Integration"), Category("AzureTable"), TestFixture,Parallelizable(ParallelScope.Children)]
public class LockGrainAzureTableIntegrationTests : LockGrainTestsBase<AzureCloudTestSiloConfigurator, AzureTestsClientConfigurator>
{
    
}