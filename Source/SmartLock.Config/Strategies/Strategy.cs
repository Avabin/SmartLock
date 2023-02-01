namespace SmartLock.Config.Strategies;

public readonly record struct Strategy(string Value)
{
    // implicit conversion from string to Strategy
    public static implicit operator Strategy(string value) => new(value);
    
    // implicit conversion from Strategy to string
    public static implicit operator string(Strategy strategy) => strategy.Value;
    
    // Redis strategy
    public const string Redis = "Redis";
    
    // local memory strategy
    public const string Local = "Local";
    
    // MongoDB strategy
    public const string Mongo = "Mongo";
    
    // Azure Table Storage strategy
    public const string AzureTable = "AzureTable";
};