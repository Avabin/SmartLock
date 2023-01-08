using Microsoft.ML.Data;
using Microsoft.ML.OnnxRuntime;
using Yolo.Core;

namespace ObjectDetector.Yolo.Onnx;

public record OnnxImageData(int Width, int Height, NamedOnnxValue Data) : ImageData<NamedOnnxValue>(Width, Height, Data)
{
    public override void Dispose()
    {
        var disposableReadOnlyCollection = Data.Value as IDisposableReadOnlyCollection<DisposableNamedOnnxValue>;
        disposableReadOnlyCollection?.Dispose();
    }
}