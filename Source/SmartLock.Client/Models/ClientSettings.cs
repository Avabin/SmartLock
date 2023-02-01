using MemoryPack;

namespace SmartLock.Client.Models;

[GenerateSerializer, Immutable, MemoryPackable]
public readonly partial record struct ClientSettings([property: Id(0)][Immutable] LocationModel? DefaultBuilding, [property: Id(1)] [Immutable] string Name, [property: Id(2)] [Immutable] string DeviceId);