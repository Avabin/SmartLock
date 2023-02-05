using SmartLock.Config.Strategies;
using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.Grains.BuildingGrain;

[Category("Integration"), Category(Strategy.Mongo), TestFixture,Parallelizable(ParallelScope.Children)]
public class BuildingGrainIntegrationMongoTests : BuildingGrainTestsBase<MongoTestSiloBuilderConfigurator, MongoTestsClientConfigurator>
{
}