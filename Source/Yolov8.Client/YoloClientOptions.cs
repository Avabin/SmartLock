namespace Yolov8.Client;

public class YoloClientOptions
{
    public string Protocol { get; set; } = "grpc";
    public string BaseUrl { get; set; } = "localhost:50051";
}