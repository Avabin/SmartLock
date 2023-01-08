using Microsoft.ML.OnnxRuntime;

namespace ObjectDetector.Yolo.Onnx;

public readonly record struct ImageData(int Width, int Height, NamedOnnxValue Data);