using System.Drawing;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using Emgu.CV;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.Transforms.Image;
using ObjectDetector;
using ObjectDetector.Yolo.Onnx;
using SkiaSharp;
using Tensorflow;
using Tensorflow.NumPy;

namespace ImageRunner;

public class ImageRunner : ConsoleAppBase
{
    private readonly ILogger<ImageRunner> _logger;

    private readonly IDictionary<string, SKColor> _labelColors;

    public ImageRunner(ILogger<ImageRunner> logger)
    {
        _logger = logger;
        
        _labelColors = new Dictionary<string, SKColor>();
    }
    [Command("detect","Recognize objects in an image")]
    public async Task RunAsync([Option("i", "video url")] string videoUrl, CancellationToken token = default)
    {
        // var (modelWidth, modelHeight) = ObjectDetector.GetInputSize();
        // var videoCapture = new VideoCapture(videoUrl);
        // // var videoCapture = new VideoCapture("http://localhost:8084");
        //
        // // create a subject to publish frames to
        // var subject = new Subject<Mat>();
        // int height =(int) videoCapture.Get(Emgu.CV.CvEnum.CapProp.FrameHeight);
        // int width = (int)videoCapture.Get(Emgu.CV.CvEnum.CapProp.FrameWidth);
        //
        // videoCapture.ImageGrabbed += (sender, args) =>
        // {
        //     var frane = new Mat();
        //     videoCapture.Retrieve(frane);
        //     subject.OnNext(frane);
        // };

        // subject.AsObservable()
        //     .Window(TimeSpan.FromSeconds(1))
        //     .SelectMany(x => x.FirstOrDefaultAsync())
        //     .Where(x => x is not null)
        //     .Timestamp()
        //     .Do(x => _logger.LogInformation("Processing frame at {FrameTime:O}", x.Timestamp))
        //     .Select(x => x.Value)
        //     .Select(x =>
        //     {
        //         var frame = new Mat();
        //         CvInvoke.Resize(x, frame, new Size(modelWidth, modelHeight), 0, 0, Emgu.CV.CvEnum.Inter.Lanczos4);
        //         x.Dispose();
        //         return frame;
        //     })
        //     .Select(Utils.ExtractPixels!)
        //     .ObserveOn(TaskPoolScheduler.Default)
        //     .Timestamp()
        //     .Do(x => _logger.LogInformation("Extracted pixels at {FrameTime:O}", x.Timestamp))
        //     .Select(x => x.Value)
        //     .Select(x => ObjectDetector.DetectAsync(new OnnxImageData(width, height, NamedOnnxValue.CreateFromTensor("images", x))).ToObservable())
        //     .Concat()
        //     .Do(x =>
        //     {
        //         _logger.LogInformation("Detected {Count} objects", x.Predictions.Count());
        //         foreach (var prediction in x.Predictions)
        //         {
        //             _logger.LogInformation("Detected {Label} at {@Box}", prediction.Label, prediction.BoundingBox);
        //         }
        //     })
        //     .Subscribe();


        _logger.LogInformation("Constructing pipeline");
        // var modelPath = @"F:\repos\ONNX-YOLOv7-Object-Detection\models\yolov7-tiny_256x320.onnx";
        var modelPath = @"C:\Users\avabi\PycharmProjects\yolov7\yolov7-e6e.onnx";
        var mlContext = new MLContext()
        {
            FallbackToCpu = true
        };
        var modelInput = new ModelInput()
        {
            ImagePath = @"C:\Users\avabi\Pictures\Zrzut ekranu 2022-12-24 132711.png"
        };
        var modelWidth = 1280;
        var modelHeight = 1280;
        var shapeDictionary = new Dictionary<string, int[]>()
        {
            {"images", new[] {1, 3, modelHeight, modelWidth}},
            {"det_classes", new[] {1}},
            {"det_scores", new[] {1}},
            {"det_boxes", new[] {1}},
            {"num_dets", new[] {1}}
        };
        var pipeline = mlContext.Transforms.LoadImages("images", "", "images")
            .Append(mlContext.Transforms.ResizeImages("images", modelWidth, modelHeight, "images"))
            .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "images", inputColumnName: "images"));

        var yolo = mlContext.Transforms.ApplyOnnxModel(modelPath, gpuDeviceId: 0, fallbackToCpu: false,
            shapeDictionary: shapeDictionary);
            
        // var pipeline = mlContext.Transforms.ApplyOnnxModel(modelPath, gpuDeviceId: 0, fallbackToCpu: true);

        _logger.LogInformation("Creating prediction engine");

        // train 
        var model = pipeline.Fit(mlContext.Data.LoadFromEnumerable(new List<ModelInput>()));
        var predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ImageOutput>(model);

        var modelInputs = Directory.GetFiles(@"F:\repos\OIDv4_ToolKit\OID\Dataset\test\dog", "*.jpg")
            .Concat(Directory.GetFiles(@"F:\repos\OIDv4_ToolKit\OID\Dataset\test\dog", "*.png"))
            .Select(x => new ModelInput()
            {
                ImagePath = x
            });

        var predictions = modelInputs.Select(x => predictionEngine.Predict(x)).ToList();
    
    }

}

public record ModelInput
{
    [ColumnName("images")]
    public string ImagePath { get; set; }
}

public record ImageOutput
{
    [ColumnName("images")] public byte[] Pixels { get; set; }
}
public record ModelOutput
{
    [ColumnName("num_dets")]
    public int NumberOfDetections { get; set; }
    [ColumnName("det_boxes")]
    public float[] Boxes { get; set; }
    [ColumnName("det_scores")]
    public float[] Scores { get; set; }
    [ColumnName("det_classes")]
    public float[] Classes { get; set; }
}

public record OnnxImagePrediction
{
}

public record ImageDetectionResult(SingleImagePrediction Prediction, SKBitmap Bitmap, string ImagePath);