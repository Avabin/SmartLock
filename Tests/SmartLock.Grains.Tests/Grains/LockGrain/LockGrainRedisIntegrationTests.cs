using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.LockGrain;

[Category("Integration"), Category("Redis"), TestFixture, Parallelizable(ParallelScope.Children)]
public class LockGrainRedisIntegrationTests : LockGrainTestsBase<RedisTestSiloBuilderConfigurator, RedisTestsClientConfigurator>
{
    
}