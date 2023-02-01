using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.WebApi.Mongo;

// mongo
[TestFixture, Category("Mongo"), Category("WebApi")]
public class SmartLockWebApiMongoTests : SmartLockWebApiTestsBase<MongoTestSiloBuilderConfigurator, MongoTestsClientConfigurator, Mongo.Program>
{
    
}