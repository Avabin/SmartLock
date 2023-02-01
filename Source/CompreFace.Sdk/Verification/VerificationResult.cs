using CompreFace.Sdk.Detection.Models;
using Newtonsoft.Json;

namespace CompreFace.Sdk.Verification;

public record VerificationResult(
    [property: JsonProperty("source_image_face")] SourceImageFace SourceImageFace,
    [property: JsonProperty("face_matches")] IReadOnlyList<FaceMatch> FaceMatches,
    [property: JsonProperty("plugins_versions")] PluginsVersions PluginsVersions
);