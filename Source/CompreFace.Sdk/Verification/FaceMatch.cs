using CompreFace.Sdk.Detection.Models;
using Newtonsoft.Json;

namespace CompreFace.Sdk.Verification;

public record FaceMatch(
    [property: JsonProperty("box")] Box Box,
    [property: JsonProperty("similarity")] double Similarity,
    [property: JsonProperty("execution_time")] ExecutionTime ExecutionTime
);