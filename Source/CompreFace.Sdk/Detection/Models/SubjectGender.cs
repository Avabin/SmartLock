using Newtonsoft.Json;

namespace CompreFace.Sdk.Detection.Models;

public record SubjectGender(
    [property: JsonProperty("probability")]
    int Probability,
    [property: JsonProperty("value")] string Value
);