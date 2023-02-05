using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.Grains.LockGrain;

[Category("Unit"), TestFixture, Parallelizable(ParallelScope.Children)]
public class LockGrainUnitTests : LockGrainTestsBase<UnitTestSiloBuilderConfigurator, UnitTestsClientConfigurator>
{
    
}