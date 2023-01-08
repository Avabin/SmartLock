using Newtonsoft.Json;

namespace CompreFace.Sdk.Detection.Models;

public record PluginsVersions(
    [property: JsonProperty("age")] string Age,
    [property: JsonProperty("gender")] string Gender,
    [property: JsonProperty("detector")] string Detector,
    [property: JsonProperty("calculator")] string Calculator,
    [property: JsonProperty("mask")] string Mask
);