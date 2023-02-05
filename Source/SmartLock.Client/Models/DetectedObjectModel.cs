using MemoryPack;

namespace SmartLock.Client.Models;

[GenerateSerializer, Immutable, MemoryPackable]
public readonly partial record struct DetectedObjectModel([property: Id(0)]string Class, [property: Id(1)] double Confidence, [property: Id(2)] long X1, [property: Id(3)] long Y1, [property: Id(4)] long X2, [property: Id(5)] long Y2);