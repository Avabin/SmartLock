using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.DetectorGrain;

[Category("Integration"), Category("Redis"), TestFixture,Parallelizable(ParallelScope.Children)]
public class DetectorGrainTestsIntegrationRedis : DetectorGrainTestsBase<RedisTestSiloBuilderConfigurator, RedisTestsClientConfigurator>
{
    
}