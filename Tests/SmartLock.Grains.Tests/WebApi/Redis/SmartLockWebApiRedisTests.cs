using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;

namespace SmartLock.Grains.Tests.WebApi.Redis;

// redis
[TestFixture, Category("Redis"), Category("WebApi")]
public class SmartLockWebApiRedisTests : SmartLockWebApiTestsBase<RedisTestSiloBuilderConfigurator, RedisTestsClientConfigurator, Redis.Program>
{
    
}