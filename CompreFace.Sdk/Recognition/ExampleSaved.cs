using Newtonsoft.Json;

namespace CompreFace.Sdk.Recognition;

public record ExampleSaved([JsonProperty("image_id")] Guid ImageId, [JsonProperty("subject")] string Subject)
{
    [JsonIgnore]
    public bool ImageAlreadyExists => ImageId == Guid.Empty;
}