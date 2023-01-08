using Newtonsoft.Json;

namespace CompreFace.Sdk;

public record DetectionMask(
    [property: JsonProperty("probability")]
    double Probability,
    [property: JsonProperty("value")] string Value
);