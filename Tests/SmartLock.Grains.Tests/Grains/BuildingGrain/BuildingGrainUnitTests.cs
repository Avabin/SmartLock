using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.BuildingGrain;

[Category("Unit"), TestFixture]
public class BuildingGrainUnitTests : BuildingGrainTestsBase<UnitTestSiloBuilderConfigurator, UnitTestsClientConfigurator>
{
}