using CompreFace.Sdk.Detection.Models;
using Newtonsoft.Json;

namespace CompreFace.Sdk.Verification;

public record SourceImageFace(
    [property: JsonProperty("box")] Box Box,
    [property: JsonProperty("execution_time")] ExecutionTime ExecutionTime
);