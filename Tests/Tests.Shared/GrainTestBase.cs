using Orleans.Streams;
using Orleans.TestingHost;
using SmartLock.Orleans.Core;

namespace Tests.Shared;

public class GrainTestBase<TSiloConfigurator, TClientConfigurator> : OrleansTestBase<TSiloConfigurator, TClientConfigurator> where TSiloConfigurator : ISiloConfigurator, new() where TClientConfigurator : IClientBuilderConfigurator, new()
{
    protected IGrainFactory GrainFactory => Cluster!.GrainFactory;
    
    protected IClusterClient Client => Cluster!.Client;

    private Lazy<IStreamProvider> _streamProvider => new(() => Client.GetStreamProvider(StreamProviderConstants.DefaultStreamProviderName));
    protected IStreamProvider DefaultStreamProvider => _streamProvider.Value;
}