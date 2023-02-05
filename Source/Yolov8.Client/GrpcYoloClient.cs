using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Yolo;

namespace Yolov8.Client;

public class GrpcYoloClient : IYoloClient, IDisposable
{
    private readonly IOptions<YoloClientOptions> _options;
    private YoloClientOptions Options => _options.Value;
    private readonly YoloDetectionService.YoloDetectionServiceClient _client;
    private readonly GrpcChannel _channel;

    public GrpcYoloClient(IOptions<YoloClientOptions> options)
    {
        _options = options;
        _channel = GrpcChannel.ForAddress(Options.BaseUrl);
        _client = new YoloDetectionService.YoloDetectionServiceClient(_channel);
    }

    public async Task<IReadOnlyList<YoloDetectionModel>> DetectAsync(Stream image,float confidence = 0.5f, float intersectionOverUnion = 0.5f)
    {
        try
        {
            var result = await _client.DetectAsync(new YoloDetectionRequest()
            {
                Image = await ByteString.FromStreamAsync(image)
            });
        
            if (result is null)
            {
                return new List<YoloDetectionModel>();
            }
        
            return result.Results.Select(detection => new YoloDetectionModel()
            {
                ClassName = detection.Label,
                Confidence = detection.Confidence,
                X1 = detection.X1,
                X2 = detection.X2,
                Y1 = detection.Y1,
                Y2 = detection.Y2
            }).ToList();
        }
        catch (RpcException e)
        {
            throw new ArgumentException("Invalid image", e);
        }
    }

    public async Task<IReadOnlyList<YoloDetectionModel>> DetectAsync(string imageUrl,float confidence = 0.5f, float intersectionOverUnion = 0.5f)
    {
        try
        {
            var result = await _client.DetectUrlAsync(new YoloDetectionUrlRequest()
            {
                Url = imageUrl,
                Params = new YoloParams
                {
                    ConfThreshold = confidence,
                    IouThreshold = intersectionOverUnion
                }
            });
        
            if (result is null)
            {
                return new List<YoloDetectionModel>();
            }
        
            return result.Results.Select(detection => new YoloDetectionModel
            {
                ClassName = detection.Label,
                Confidence = detection.Confidence,
                X1 = detection.X1,
                X2 = detection.X2,
                Y1 = detection.Y1,
                Y2 = detection.Y2
            }).ToList();
        }
        catch (RpcException e)
        {
            throw new ArgumentException("Invalid image url", e);
        }
    }

    public void Dispose()
    {
        _channel.Dispose();
    }
}