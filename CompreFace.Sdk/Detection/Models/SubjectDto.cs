using Newtonsoft.Json;

namespace CompreFace.Sdk.Detection.Models;

public record Subject(
    [property: JsonProperty("subject")] string SubjectName,
    [property: JsonProperty("similarity")] float Similarity
);