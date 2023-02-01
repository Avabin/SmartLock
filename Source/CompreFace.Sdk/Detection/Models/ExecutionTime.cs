using Newtonsoft.Json;

namespace CompreFace.Sdk.Detection.Models;

public record ExecutionTime(
    [property: JsonProperty("age")] int Age,
    [property: JsonProperty("gender")] int Gender,
    [property: JsonProperty("detector")] int Detector,
    [property: JsonProperty("calculator")] int Calculator,
    [property: JsonProperty("mask")] int Mask
);