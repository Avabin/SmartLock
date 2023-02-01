using MemoryPack;

namespace SmartLock.Client.Models;

[Immutable, GenerateSerializer, MemoryPackable]
public partial record AddLock([Immutable] [property: Id(0)] LocationModel Lock, LocationModel Building) : BuildingEvent(Building);

[Immutable, GenerateSerializer, MemoryPackable]
public partial record RemoveLock([Immutable] [property: Id(0)] LocationModel Lock, LocationModel Building) : BuildingEvent(Building);

[Immutable, GenerateSerializer, MemoryPackable]
public partial record OpenLock([Immutable] [property: Id(0)] LocationModel Location, LocationModel Building) : BuildingEvent(Building);

[Immutable, GenerateSerializer, MemoryPackable]
public partial record CloseLock([Immutable] [property: Id(0)] LocationModel Location, LocationModel Building) : BuildingEvent(Building);

[Immutable, GenerateSerializer, MemoryPackable]
public partial record OpenAllLocks(LocationModel Building) : BuildingEvent(Building);

[Immutable, GenerateSerializer, MemoryPackable]
public partial record CloseAllLocks(LocationModel Building) : BuildingEvent(Building);
