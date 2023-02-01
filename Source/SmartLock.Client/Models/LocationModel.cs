using MemoryPack;

namespace SmartLock.Client.Models;

/// <summary>
/// Building location
/// </summary>
/// <param name="Value">Location distinct name</param>
[GenerateSerializer, Immutable, MemoryPackable]
public readonly partial record struct LocationModel([Immutable] [property: Id(0)] string Value)
{
    public static implicit operator string(LocationModel location) => location.Value;
    public static implicit operator LocationModel(string location) => new(location);
}