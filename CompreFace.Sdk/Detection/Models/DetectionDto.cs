using Newtonsoft.Json;

namespace CompreFace.Sdk.Detection.Models;

public record DetectionDto(
    [property: JsonProperty("result")] IReadOnlyList<DetectionResult> Result,
    [property: JsonProperty("plugins_versions")]
    PluginsVersions DetectionPluginsVersions
)
{
    public static DetectionDto Empty => new(new List<DetectionResult>(), PluginsVersions.Empty);
}