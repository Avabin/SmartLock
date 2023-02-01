using MemoryPack;

namespace SmartLock.Client.Models;

[GenerateSerializer, Immutable, MemoryPackable]
public readonly partial record struct RegisterClientModel([property: Id(0)] string DeviceId,[property: Id(1)] string Name,[property: Id(2)] string? DefaultBuilding);