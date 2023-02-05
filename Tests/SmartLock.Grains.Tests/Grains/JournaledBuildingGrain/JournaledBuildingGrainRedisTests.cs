using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.Grains.JournaledBuildingGrain;


// redis
[Category("Integration"), Category("Redis"), TestFixture,Parallelizable(ParallelScope.Children)]
public class JournaledBuildingGrainRedisTests : JournaledBuildingGrainTestsBase<RedisTestSiloBuilderConfigurator, RedisTestsClientConfigurator>
{
    
}

// azure table