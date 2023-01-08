using CompreFace.Sdk.Detection.Models;
using Newtonsoft.Json;

namespace CompreFace.Sdk.Recognition;
public record RecognitionDto(
    [property: JsonProperty("result")] IReadOnlyList<RecognitionResult> Result,
    [property: JsonProperty("plugins_versions")]
    PluginsVersions PluginsVersions)
{
    public static RecognitionDto Empty => new(new List<RecognitionResult>(), PluginsVersions.Empty);
}

public record RecognitionResult(
    [property: JsonProperty("box")] Box Box,
    [property: JsonProperty("subjects")] IReadOnlyList<SubjectDto> Subjects,
    [property: JsonProperty("execution_time")] ExecutionTime ExecutionTime
);
