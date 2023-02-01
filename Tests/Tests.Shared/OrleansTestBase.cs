using NUnit.Framework;
using Orleans.TestingHost;
using Tests.Shared.ClientConfigurators;

namespace Tests.Shared;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract class OrleansTestBase<TSiloConfigurator, TClientConfigurator> where TSiloConfigurator : ISiloConfigurator, new() where TClientConfigurator : IClientBuilderConfigurator, new()
{
    public static TestClusterBuilder ClusterBuilder { get; } = new();
    public static TestCluster? Cluster { get; private set; } = null;
    [OneTimeSetUp]
    public static async Task OneTimeSetUp()
    {
        if (Cluster == null)
        {
            var clusterExists = Cluster is not null;
            if (!clusterExists)
            {
                Cluster = ClusterBuilder.AddSiloBuilderConfigurator<TSiloConfigurator>().AddClientBuilderConfigurator<TClientConfigurator>().Build();
                await Cluster.DeployAsync();
            }
        }

    }
    
    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        // await Cluster!.StopAllSilosAsync();
        //
        // await Cluster!.DisposeAsync();
    }
}