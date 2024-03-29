using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.Grains.JournaledBuildingGrain;

[Category("Integration"), Category("Mongo"), TestFixture,Parallelizable(ParallelScope.Children)]
public class JournaledBuildingGrainMongoIntegrationTests : JournaledBuildingGrainTestsBase<MongoTestSiloBuilderConfigurator, MongoTestsClientConfigurator>
{
    
}