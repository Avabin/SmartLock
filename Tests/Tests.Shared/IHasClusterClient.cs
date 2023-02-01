namespace Tests.Shared;

public interface IHasClusterClient
{
    static IClusterClient? Client { get; set; }
}