namespace Yolo.Core;

public interface IYoloModelProvider
{
    string GetE6EModelPath();
    string GetTinyModelPath();
}

public class YoloModelProvider : IYoloModelProvider
{
    public string GetE6EModelPath() => @"C:\Users\avabi\PycharmProjects\yolov7\yolov7-e6e.onnx";

    public string GetTinyModelPath() => @"C:\Users\avabi\PycharmProjects\yolov7\yolov7-tiny.onnx";
}