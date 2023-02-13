using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Stream.Client;
using SmartLock.Orleans.Core;
using SmartLock.Streams.RabbitMQ.RabbitMQ;

namespace SmartLock.Streams.RabbitMQ.Configurators;

public static class SiloBuilderExtensions
{
    /// <summary>
    ///     Configure silo to use RabbitMQ persistent streams.
    /// </summary>
    public static ISiloBuilder AddRabbitMQStreams(this ISiloBuilder builder, string name,
        Action<RabbitMQClientOptions> configureOptions)
    {
        builder.AddRabbitMQStreams(name, b => b.ConfigureRabbitMQ(ob => ob.Configure(configureOptions)));
        return builder;
    }

    /// <summary>
    ///     Configure silo to use RabbitMQ persistent streams.
    /// </summary>
    public static ISiloBuilder AddRabbitMQStreams(this ISiloBuilder builder, string name,
        Action<RabbitMQSiloConfigurator>? configure = null)
    {
        var configurator = new RabbitMQSiloConfigurator(name,
            configureServicesDelegate => builder.ConfigureServices(configureServicesDelegate));
        configure?.Invoke(configurator);
        return builder;
    }

    public static ISiloBuilder AddRabbitMQStreams(this ISiloBuilder builder, string name, IConfigurationSection section)
    {
        
        var rabbitHost = section.GetValue<string?>("Host") ?? "127.0.0.1";
        var rabbitPort = section.GetValue<int?>("Port") ?? 5552;
        var rabbitUser = section.GetValue<string?>("UserName") ?? "guest";
        var rabbitPassword = section.GetValue<string?>("Password") ?? "guest";
        var rabbitVirtualHost = section.GetValue<string?>("VirtualHost") ?? "/";
        var rabbitClientName = section.GetValue<string?>("ClientName") ?? $"SmartLock-{Random.Shared.NextInt64():X}";
        EndPoint endpoint = IPAddress.TryParse(rabbitHost, out _)
            ? IPEndPoint.Parse($"{rabbitHost}:{rabbitPort}")
            : new DnsEndPoint(rabbitHost, rabbitPort);
        builder.AddRabbitMQStreams(name, options =>
        {
            options.StreamSystemConfig = new StreamSystemConfig
            {
                Endpoints = new List<EndPoint> {endpoint},
                AddressResolver = new AddressResolver(endpoint),
                ClientProvidedName = rabbitClientName,
                VirtualHost = rabbitVirtualHost,
                Password = rabbitPassword,
                UserName = rabbitUser,
                Ssl = new SslOption() {Enabled = false}
            };
        });

        return builder;
    }

    public static void AddRabbitMQStreams(this ISiloBuilder builder, IConfigurationSection section) =>
        AddRabbitMQStreams(builder, StreamProviderConstants.DefaultStreamProviderName, section);
}