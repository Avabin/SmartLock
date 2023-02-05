using System.Collections.Immutable;

namespace SmartLock.Client.Models;

[Immutable, GenerateSerializer
]
public readonly record struct DetectionRequest([property: Id(0)] ImmutableArray<byte> Data);