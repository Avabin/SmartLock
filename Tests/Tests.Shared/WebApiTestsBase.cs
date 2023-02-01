using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Orleans.TestingHost;
using RabbitMQ.Stream.Client.AMQP;
using SmartLock.Client.HttpClient;

namespace Tests.Shared;

public abstract class WebApiTestsBase<TSilo, TClient, TEntrypoint> : OrleansTestBase<TSilo, TClient> where TSilo : ISiloConfigurator, new() where TEntrypoint : class, IHasClusterClient where TClient : IClientBuilderConfigurator, new()
{
    protected static TestServer Server { get; private set; } = null!;
    protected static WebApplicationFactory<TEntrypoint> WebApplicationFactory { get; private set; } = null!;
    protected HttpClient Client => WebApplicationFactory.CreateClient();
    
    [OneTimeSetUp]
    public new static async Task OneTimeSetUp()
    {
        // set ASPNET_ENVIRONMENT to Tests to enable logging
        await OrleansTestBase<TSilo, TClient>.OneTimeSetUp();
        var clusterClient = Cluster!.Client;
        IHasClusterClient.Client = clusterClient;

        var waf = new WebApplicationFactory<TEntrypoint>();

        WebApplicationFactory = waf;
        Server = waf.Server;
    }
    
    [OneTimeTearDown]
    public static async Task TearDown()
    {
        await OrleansTestBase<TSilo, TClient>.OneTimeTearDown();
        await WebApplicationFactory.DisposeAsync();
    }
}