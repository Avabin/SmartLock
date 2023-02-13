using System.Diagnostics;
using MemoryPack;

namespace SmartLock.Client.Models;

[Immutable, GenerateSerializer]
[MemoryPackable]
[MemoryPackUnion(0, typeof(AddLock))]
[MemoryPackUnion(1, typeof(RemoveLock))]
[MemoryPackUnion(2, typeof(OpenAllLocks))]
[MemoryPackUnion(3, typeof(CloseAllLocks))]
[MemoryPackUnion(4, typeof(OpenLock))]
[MemoryPackUnion(5, typeof(CloseLock))]
public abstract partial record BuildingEvent([property: Id(0)] LocationModel Building, [property: Id(1)] string? TraceParent,[property: Id(2)] string? TraceState) : IEvent
{
}