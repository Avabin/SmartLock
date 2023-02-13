using System.Net;
using Microsoft.Extensions.Configuration;
using SmartLock.Streams.RabbitMQ.RabbitMQ;

namespace SmartLock.Streams.RabbitMQ.Configurators;

public static class ClientBuilderExtensions
{
    /// <summary>
    ///     Configure client to use RabbitMQ persistent streams.
    /// </summary>
    public static IClientBuilder AddRabbitMQStreams(this IClientBuilder builder, string name,
        Action<RabbitMQClientOptions> configureOptions)
    {
        builder.AddRabbitMQStreams(name, b => b.ConfigureRabbitMQ(ob => ob.Configure(configureOptions)));
        return builder;
    }

    /// <summary>
    ///     Configure client to use RabbitMQ persistent streams.
    /// </summary>
    public static IClientBuilder AddRabbitMQStreams(this IClientBuilder builder, string name,
        Action<RabbitMQClientConfigurator>? configure = null)
    {
        var configurator = new RabbitMQClientConfigurator(name, builder);
        configure?.Invoke(configurator);
        return builder;
    }

    public static IClientBuilder AddRabbitMQStreams(this IClientBuilder builder, string name, IConfigurationSection section)
    {
        
        var rabbitHost = section.GetValue<string?>("Host") ?? "127.0.0.1";
        var rabbitPort = section.GetValue<int?>("Port") ?? 5672;
        var rabbitUser = section.GetValue<string?>("UserName") ?? "guest";
        var rabbitPassword = section.GetValue<string?>("Password") ?? "guest";
        var rabbitVirtualHost = section.GetValue<string?>("VirtualHost") ?? "/";
        var rabbitClientName = section.GetValue<string?>("ClientName") ?? $"SmartLock-{Random.Shared.NextInt64():X}";
        EndPoint endpoint = IPAddress.TryParse(rabbitHost, out _)
            ? IPEndPoint.Parse($"{rabbitHost}:{rabbitPort}")
            : new DnsEndPoint(rabbitHost, rabbitPort);
        builder.AddRabbitMQStreams(name, options =>
        {
            options.StreamSystemConfig.Endpoints.Clear();
            options.StreamSystemConfig.Endpoints.Add(endpoint);
            options.StreamSystemConfig.VirtualHost = rabbitVirtualHost;
            options.StreamSystemConfig.UserName = rabbitUser;
            options.StreamSystemConfig.Password = rabbitPassword;
            options.StreamSystemConfig.ClientProvidedName = rabbitClientName;
        });

        return builder;
    }
}