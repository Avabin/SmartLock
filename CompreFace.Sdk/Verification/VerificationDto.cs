using Newtonsoft.Json;

namespace CompreFace.Sdk.Verification;

public record VerificationDto(
    [property: JsonProperty("result")] IReadOnlyList<VerificationResult> Result
);