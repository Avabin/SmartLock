using Newtonsoft.Json;

namespace CompreFace.Sdk.Detection.Models;

public record SubjectDto(
    [property: JsonProperty("subject")] string Subject,
    [property: JsonProperty("similarity")] float Similarity
);