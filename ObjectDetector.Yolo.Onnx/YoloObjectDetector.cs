using System.Diagnostics;
using System.Drawing;
using Microsoft.Extensions.Options;
using SkiaSharp;

namespace Yolov7;

public class YoloObjectDetector : IObjectDetector, IDisposable
{
    private readonly IYoloModelProvider _yoloModelProvider;
    private readonly IOptions<ObjectDetectorOptions> _options;
    private ObjectDetectorOptions Options => _options.Value;
    private readonly Yolov7.Yolov7 _yolov7;

    public YoloObjectDetector(IYoloModelProvider yoloModelProvider, IOptions<ObjectDetectorOptions> options)
    {
        _yoloModelProvider = yoloModelProvider;
        _options = options;

        _yolov7 = new Yolov7.Yolov7(_yoloModelProvider.GetE6EModelPath(), Options.UseCuda);
        _yolov7.SetupYoloDefaultLabels();
    }
    public async Task<(TimeSpan elapsed, List<DetectionResult> predictions)> DetectAsync(SKBitmap bitmap)
    {
        var result = await Task.Run(() =>
        { 
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var predictions = _yolov7.Predict(bitmap);
            stopwatch.Stop();
            return (stopwatch.Elapsed, predictions);
        });
        return (result.Elapsed, result.predictions.Select(x => new DetectionResult(x.Label?.Name ?? "", x.Score, x.Rectangle)).ToList());
    }

    public void Dispose()
    {
        _yolov7.Dispose();
    }
}