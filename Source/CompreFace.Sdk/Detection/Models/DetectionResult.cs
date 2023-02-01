using Newtonsoft.Json;

namespace CompreFace.Sdk.Detection.Models;

public record DetectionResult(
    [property: JsonProperty("age")] SubjectAge? DetectionSubjectAge,
    [property: JsonProperty("gender")] SubjectGender? DetectionSubjectGender,
    [property: JsonProperty("embedding")] IReadOnlyList<double> Embedding,
    [property: JsonProperty("box")] Box Box,
    [property: JsonProperty("mask")] DetectionMask DetectionMask,
    [property: JsonProperty("subjects")] IReadOnlyList<SubjectDto> Subjects,
    [property: JsonProperty("landmarks")] IReadOnlyList<List<int>> Landmarks,
    [property: JsonProperty("execution_time")]
    ExecutionTime ExecutionTime
);