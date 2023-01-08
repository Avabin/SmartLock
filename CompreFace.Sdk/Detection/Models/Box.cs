using Newtonsoft.Json;

namespace CompreFace.Sdk.Detection.Models;

public record DetectionBox(
    [property: JsonProperty("probability")]
    double Probability,
    [property: JsonProperty("x_max")] int XMax,
    [property: JsonProperty("y_max")] int YMax,
    [property: JsonProperty("x_min")] int XMin,
    [property: JsonProperty("y_min")] int YMin
);