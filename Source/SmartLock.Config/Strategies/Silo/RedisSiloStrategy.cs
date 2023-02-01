using System.Net;
using System.Net.Sockets;
using SmartLock.Streams.RabbitMQ.Configurators;

namespace SmartLock.Config.Strategies.Silo;

public class RedisSiloStrategy : SiloStrategyBase
{
    public override void ConfigureSilo(ISiloBuilder builder, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");
        var siloPort = configuration.GetValue<int?>("Orleans:SiloPort") ?? 11111;
        var ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork); 
        builder.ConfigureEndpoints(ipAddress, siloPort, 30000);
        builder.UseRedisClustering(options =>
        {
            options.Database = 1;
            options.ConnectionString = redisConnectionString ?? throw new InvalidOperationException("Redis connection string is not set");
        });
        builder.AddRedisGrainStorageAsDefault();
        builder.AddRedisGrainStorage("PubSubStore");
        builder.AddReminders();
        builder.AddRabbitMQStreams(configuration.GetRequiredSection("RabbitMQ"));
        builder.AddLogStorageBasedLogConsistencyProvider();
    }
}