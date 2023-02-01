namespace CompreFace.Sdk.Verification;

public interface IVerificationClient
{
    Task<VerificationDto> VerifyAsync(Func<Stream> sourceImage, Func<Stream> targetImage, int limit = 10, float threshold = 0.5f, string facePlugins = "all", bool includeSystemInfo = false);
}