using Newtonsoft.Json;

namespace CompreFace.Sdk;

public record DetectionResult(
    [property: JsonProperty("age")] DetectionSubjectAge? DetectionSubjectAge,
    [property: JsonProperty("gender")] DetectionSubjectGender? DetectionSubjectGender,
    [property: JsonProperty("embedding")] IReadOnlyList<double> Embedding,
    [property: JsonProperty("box")] DetectionBox DetectionBox,
    [property: JsonProperty("mask")] DetectionMask DetectionMask,
    [property: JsonProperty("subjects")] IReadOnlyList<DetectionSubject> Subjects,
    [property: JsonProperty("landmarks")] IReadOnlyList<List<int>> Landmarks,
    [property: JsonProperty("execution_time")]
    DetectionExecutionTime DetectionExecutionTime
);