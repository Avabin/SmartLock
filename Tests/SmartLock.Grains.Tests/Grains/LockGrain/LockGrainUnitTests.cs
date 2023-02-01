using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.LockGrain;

[Category("Unit"), TestFixture, Parallelizable(ParallelScope.Children)]
public class LockGrainUnitTests : LockGrainTestsBase<UnitTestSiloBuilderConfigurator, UnitTestsClientConfigurator>
{
    
}