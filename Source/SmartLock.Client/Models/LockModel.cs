using MemoryPack;

namespace SmartLock.Client.Models;


[Immutable, GenerateSerializer, MemoryPackable]
public readonly record struct LockModel([property: Id(0)] LocationModel Location, [property: Id(1)] bool IsOpen);