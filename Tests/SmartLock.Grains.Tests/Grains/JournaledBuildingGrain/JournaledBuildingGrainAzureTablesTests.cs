using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.Grains.JournaledBuildingGrain;

[Category("Integration"), Category("AzureTable"), TestFixture,Parallelizable(ParallelScope.Children)]
public class JournaledBuildingGrainAzureTablesTests : JournaledBuildingGrainTestsBase<AzureCloudTestSiloConfigurator, AzureTestsClientConfigurator>
{
    
}