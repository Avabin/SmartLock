using MemoryPack;

namespace SmartLock.Client.Models;

[MemoryPackable]
[MemoryPackUnion(0, typeof(BuildingEvent))]
[MemoryPackUnion(1, typeof(AddLock))]
[MemoryPackUnion(2, typeof(RemoveLock))]
[MemoryPackUnion(3, typeof(OpenAllLocks))]
[MemoryPackUnion(4, typeof(CloseAllLocks))]
[MemoryPackUnion(5, typeof(OpenLock))]
[MemoryPackUnion(6, typeof(CloseLock))]
public partial interface IEvent
{}