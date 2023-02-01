using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.WebApi.AzureTables;

// mongo
[TestFixture, Category("AzureTable"), Category("WebApi")]
public class SmartLockWebApiAzureTablesTests : SmartLockWebApiTestsBase<MongoTestSiloBuilderConfigurator, AzureTestsClientConfigurator,AzureTables.Program>
{
    
}