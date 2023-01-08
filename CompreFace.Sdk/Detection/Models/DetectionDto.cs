using Newtonsoft.Json;

namespace CompreFace.Sdk;

public record DetectionDto(
    [property: JsonProperty("result")] IReadOnlyList<DetectionResult> Result,
    [property: JsonProperty("plugins_versions")]
    DetectionPluginsVersions DetectionPluginsVersions
);