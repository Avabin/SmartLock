namespace SmartLock.Orleans.Core;

/// <summary>
/// Value type that represents a storage provider.
/// </summary>
/// <param name="Value">The name of the storage provider.</param>
public readonly record struct StorageProvider(string Value)
{
    // implicit conversion from string
    public static implicit operator StorageProvider(string value) => new(value);
    // implicit conversion to string
    public static implicit operator string(StorageProvider provider) => provider.Value;
    
    /// <summary>
    /// The in-memory storage provider.
    /// </summary>
    public static StorageProvider Memory => "Memory";
    /// <summary>
    /// The Redis storage provider.
    /// </summary>
    public static StorageProvider Redis => "Redis";
}