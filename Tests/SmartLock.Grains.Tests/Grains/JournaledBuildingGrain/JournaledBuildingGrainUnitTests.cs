using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.Grains.JournaledBuildingGrain;

[Category("Unit"), TestFixture]
public class JournaledBuildingGrainUnitTests : JournaledBuildingGrainTestsBase<UnitTestSiloBuilderConfigurator, UnitTestsClientConfigurator>
{
    
}