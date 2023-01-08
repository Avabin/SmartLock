using Newtonsoft.Json;

namespace CompreFace.Sdk.Detection.Models;

public record DetectionMask(
    [property: JsonProperty("probability")]
    double Probability,
    [property: JsonProperty("value")] string Value
);