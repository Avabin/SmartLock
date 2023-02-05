using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.Grains.BuildingGrain;

[Category("Unit"), TestFixture]
public class BuildingGrainUnitTests : BuildingGrainTestsBase<UnitTestSiloBuilderConfigurator, UnitTestsClientConfigurator>
{
}