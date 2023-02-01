using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.DetectorGrain;

[Category("Unit"), TestFixture, Parallelizable(ParallelScope.Children)]
public class DetectorGrainTestsUnit : DetectorGrainTestsBase<UnitTestSiloBuilderConfigurator, UnitTestsClientConfigurator>
{
    
}