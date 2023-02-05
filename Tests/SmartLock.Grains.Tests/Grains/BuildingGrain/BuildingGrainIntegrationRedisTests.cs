using SmartLock.Config.Strategies;
using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.Grains.BuildingGrain;

[Category("Integration"), Category(Strategy.Redis), TestFixture,Parallelizable(ParallelScope.Children)]
public class BuildingGrainIntegrationRedisTests : BuildingGrainTestsBase<RedisTestSiloBuilderConfigurator, RedisTestsClientConfigurator>
{
}