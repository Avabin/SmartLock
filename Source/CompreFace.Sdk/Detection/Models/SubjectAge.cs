using Newtonsoft.Json;

namespace CompreFace.Sdk.Detection.Models;

public record SubjectAge(
    [property: JsonProperty("probability")]
    double Probability,
    [property: JsonProperty("high")] int High,
    [property: JsonProperty("low")] int Low
);