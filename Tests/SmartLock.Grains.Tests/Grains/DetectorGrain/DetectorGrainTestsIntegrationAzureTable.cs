using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.Grains.DetectorGrain;

[Category("Integration"), Category("AzureTable"), TestFixture, Parallelizable(ParallelScope.Children)]
public class DetectorGrainTestsIntegrationAzureTable : DetectorGrainTestsBase<AzureCloudTestSiloConfigurator, AzureTestsClientConfigurator>
{
    
}