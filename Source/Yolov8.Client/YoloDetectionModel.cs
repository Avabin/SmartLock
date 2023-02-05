using System.Text.Json.Serialization;

namespace Yolov8.Client;

public readonly record struct YoloDetectionModel([property: JsonPropertyName("class_name")] string ClassName, [property: JsonPropertyName("confidence")] double Confidence, [property: JsonPropertyName("x1")] long X1, [property: JsonPropertyName("y1")] long Y1, [property: JsonPropertyName("x2")] long X2, [property: JsonPropertyName("y2")] long Y2);