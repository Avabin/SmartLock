using System.Diagnostics;
using Microsoft.Extensions.Options;
using Yolo.Core;

namespace ObjectDetector.Yolo.Onnx;

public class YoloObjectDetector : IObjectDetector<OnnxImageData>, IDisposable
{
    private readonly IYoloModelProvider _yoloModelProvider;
    private readonly IOptions<ObjectDetectorOptions> _options;
    private ObjectDetectorOptions Options => _options.Value;
    public Yolov7 Yolov7 => _yolov7.Value;
    private readonly Lazy<Yolov7> _yolov7;

    public YoloObjectDetector(IYoloModelProvider yoloModelProvider, IOptions<ObjectDetectorOptions> options)
    {
        _yoloModelProvider = yoloModelProvider;
        _options = options;

        _yolov7 = new Lazy<Yolov7>(() =>
        {
            var yolo = new Yolov7(_yoloModelProvider.GetE6EModelPath(), Options.UseCuda);
            yolo.SetupYoloDefaultLabels();
            return yolo;
        });
    }
    public async Task<SingleImagePrediction> DetectAsync(OnnxImageData image)
    {
        var result = await Task.Run(() =>
        { 
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var predictions = Yolov7.Predict(image);
            stopwatch.Stop();
            return (stopwatch.Elapsed, predictions);
        });
        
        return new SingleImagePrediction(result.Elapsed, result.predictions.Select(x => new DetectionResult(x.Label?.Name ?? "", x.Score, x.Rectangle)).ToList());
    }

    public async Task<MultipleImagePrediction> DetectAsync(IEnumerable<OnnxImageData> images)
    {
        var result = await Task.Run(() =>
                { 
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var predictions = Yolov7.Predict(images);
                    stopwatch.Stop();
                    return (stopwatch.Elapsed, predictions);
                });
        
        var data = result.predictions.Select(x => (result.Elapsed, x.Select(y => new DetectionResult(y.Label?.Name ?? "", y.Score, y.Rectangle)).ToList())).ToList();
        var elapsed = data.Select(x => x.Item1).Aggregate(TimeSpan.Zero, (a, b) => a + b);
        return new MultipleImagePrediction(elapsed, data.Select(x => new SingleImagePrediction(x.Elapsed, x.Item2)).ToList());
    }

    public void Dispose()
    {
        Yolov7.Dispose();
    }

    public async Task<SingleImagePrediction> DetectAsync(IImageData image)
    {
        if (image is OnnxImageData data) return await DetectAsync(data);

        else
            throw new ArgumentException("Image data is not in the correct format, supported format is OnnxImageData");
    }

    public async Task<MultipleImagePrediction> DetectAsync(IEnumerable<IImageData> images)
    {
        return await DetectAsync(images.OfType<OnnxImageData>().ToList());
    }

    public (int Width, int Height) GetInputSize()
    {
        var modelWidth = Yolov7.Model.Width;
        var modelHeight = Yolov7.Model.Height;
        
        return (modelWidth, modelHeight);
    }

    public IReadOnlyCollection<string> GetLabels() => 
        Yolov7.Model.Labels.Select(x => x.Name).Where(x => !string.IsNullOrEmpty(x)).ToList()!;
}