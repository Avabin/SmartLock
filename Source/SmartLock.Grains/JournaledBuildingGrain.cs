using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Orleans.EventSourcing;
using Orleans.Streams;
using SmartLock.Client.Models;
using SmartLock.GrainInterfaces;
using SmartLock.Orleans.Core;

namespace SmartLock.Grains;

[SuppressMessage("Performance", "CA1822:Oznaczaj elementy członkowskie jako statyczne")]
public class JournaledBuildingGrain : JournaledGrain<JournaledBuildingState, BuildingEvent>, IJournaledBuildingGrain
{
    private readonly ILogger<JournaledBuildingGrain> _logger;
    private IAsyncStream<Notification<IEvent>>? _stream = null;
    private LocationModel Location => new(this.GetPrimaryKeyString());

    public JournaledBuildingGrain(ILogger<JournaledBuildingGrain> logger)
    {
        _logger = logger;
    }
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _stream = this.GetStreamProvider(StreamProviderConstants.DefaultStreamProviderName)
            .GetStream<Notification<IEvent>>(Location.ToNotificationsStreamId());
        await base.OnActivateAsync(cancellationToken);

    }
    public async ValueTask AddLockAsync(LocationModel location)
    {
        if (State.Locks.ContainsKey(location)) return;
        _logger.LogInformation("Add lock {Location}", location);
        var addLock = new AddLock(location, Location);
        RaiseEvent(addLock);
        await ConfirmEvents();
        
        _logger.LogTrace("Lock {Location} added", location);
        await NotifyAsync(addLock);
    }

    public async ValueTask RemoveLockAsync(LocationModel location)
    {
        if (!State.Locks.ContainsKey(location)) return;
        _logger.LogInformation("Remove lock {Location}", location);
        var removeLock = new RemoveLock(location, Location);
        RaiseEvent(removeLock);
        await ConfirmEvents();
        
        _logger.LogTrace("Lock {Location} removed", location);
        await NotifyAsync(removeLock);
    }

    public async ValueTask<LockModel?> GetLockAsync(LocationModel location) => 
        State.Locks.TryGetValue(location, out var lockModel) 
            ? lockModel
            : null;

    public async  ValueTask UnlockAsync(LocationModel location)
    {
        if(!State.Locks.ContainsKey(location)) return;
        _logger.LogInformation("Unlock {Location}", location);
        var evt = new OpenLock(location, Location);
        RaiseEvent(evt);
        await ConfirmEvents();
        
        _logger.LogTrace("Lock {Location} unlocked", location);
        await NotifyAsync(evt);
    }

    public async ValueTask LockAsync(LocationModel location)
    {
        if(!State.Locks.ContainsKey(location)) return;
        _logger.LogInformation("Lock {Location}", location);
        var evt = new CloseLock(location, Location);
        RaiseEvent(evt);
        await ConfirmEvents();
        
        _logger.LogTrace("Lock {Location} locked", location);
        await NotifyAsync(evt);
    }

    public async ValueTask UnlockAllAsync()
    {
        _logger.LogInformation("Unlock all");
        var evt = new OpenAllLocks(Location);
        _logger.LogDebug("Unlocking {Count} locks", State.Locks.Count);
        RaiseEvent(evt);
        await ConfirmEvents();
        
        _logger.LogTrace("All locks unlocked");
        await NotifyAsync(evt);
    }

    public async ValueTask LockAllAsync()
    {
        _logger.LogInformation("Lock all");
        var evt = new CloseAllLocks(Location);
        _logger.LogDebug("Locking {Count} locks", State.Locks.Count);
        RaiseEvent(evt);
        await ConfirmEvents();
        
        _logger.LogTrace("All locks locked");
        await NotifyAsync(evt);
    }

    public Task<LocationModel> GetBuildingLocationAsync()
    {
        return Task.FromResult(new LocationModel(this.GetPrimaryKeyString()));
    }

    public Task<IReadOnlyList<LockModel>> GetLocksAsync()
    {
        return Task.FromResult<IReadOnlyList<LockModel>>(State.Locks.Values.ToList());
    }

    public Task<IReadOnlyList<LockModel>> GetLocksAsync(params LocationModel[] locks)
    {
        return Task.FromResult<IReadOnlyList<LockModel>>(State.Locks.Values.Where(x => locks.Contains(x.Location)).ToList());
    }

    private async ValueTask NotifyAsync(IEvent @event)
    {
        var name = @event.GetType().Name;
        _logger.LogInformation("Notifying about {Event}", name);
        await _stream!.OnNextAsync(Notification.Of(@event));
        
        _logger.LogTrace("Notified about {Event}", name);
    }
}

[GenerateSerializer]
public class JournaledBuildingState
{
    [Id(0)] public Dictionary<LocationModel, LockModel> Locks { get; set; } = new();

    public async Task Apply(AddLock addLock)
    {
        Locks.Add(addLock.Lock, new LockModel(addLock.Lock, false));
    }
    
    public async Task Apply(OpenLock openLock)
    {
        var @lock = Locks[openLock.Location];
        Locks.Remove(openLock.Location);
        Locks.Add(openLock.Location, @lock with { IsOpen = true });
    }
    
    public async Task Apply(CloseLock closeLock)
    {
        var @lock = Locks[closeLock.Location];
        Locks.Remove(closeLock.Location);
        Locks.Add(closeLock.Location, @lock with { IsOpen = false });
    }

    public async Task Apply(OpenAllLocks openAllLocks)
    {
        var locks = Locks.Select(x => new KeyValuePair<LocationModel, LockModel>(x.Key, x.Value with { IsOpen = true }));
        Locks = new Dictionary<LocationModel, LockModel>(locks);
    }
    
    public async Task Apply(CloseAllLocks closeAllLocks)
    {
        var locks = Locks.Select(x => new KeyValuePair<LocationModel, LockModel>(x.Key, x.Value with { IsOpen = false }));
        Locks = new Dictionary<LocationModel, LockModel>(locks);
    }

    public async Task Apply(RemoveLock removeLock)
    {
        Locks.Remove(removeLock.Lock);
    }
};

