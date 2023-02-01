using CompreFace.Sdk.Detection;
using CompreFace.Sdk.Recognition;
using CompreFace.Sdk.Verification;

namespace CompreFace.Sdk;

public interface ICompreFaceClient
{
}

internal class CompreFaceClient : ICompreFaceClient
{
    private IDetectionClient Detection { get; }
    private IRecognitionClient Recognition { get; }
    private IVerificationClient Verification { get; }

    public CompreFaceClient(IDetectionClient detection, IRecognitionClient recognition, IVerificationClient verification)
    {
        Detection = detection;
        Recognition = recognition;
        Verification = verification;
    }
}