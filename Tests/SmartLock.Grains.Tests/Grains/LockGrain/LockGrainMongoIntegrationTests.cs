using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.LockGrain;

[Category("Integration"), Category("Mongo"), TestFixture, Parallelizable(ParallelScope.Children)]
public class LockGrainMongoIntegrationTests : LockGrainTestsBase<MongoTestSiloBuilderConfigurator, MongoTestsClientConfigurator>
{
    
}