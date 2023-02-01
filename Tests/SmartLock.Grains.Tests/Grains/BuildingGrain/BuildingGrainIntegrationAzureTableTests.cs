using SmartLock.Config.Strategies;
using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.BuildingGrain;

[Category("Integration"), Category(Strategy.AzureTable), TestFixture,Parallelizable(ParallelScope.Children)]
public class BuildingGrainIntegrationAzureTableTests : BuildingGrainTestsBase<AzureCloudTestSiloConfigurator, AzureTestsClientConfigurator>
{
}