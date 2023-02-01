using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.WebApi.Unit;

[TestFixture, Category("Unit"), Category("WebApi")]
public class SmartLockWebApiUnitTests : SmartLockWebApiTestsBase<UnitTestSiloBuilderConfigurator, UnitTestsClientConfigurator, Unit.Program>
{
    
}