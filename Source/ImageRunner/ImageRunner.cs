using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using ObjectDetector;
using SkiaSharp;
using Tensor = Tensorflow.Tensor;

namespace ImageRunner;

public class ImageRunner : ConsoleAppBase
{
    private readonly ILogger<ImageRunner> _logger;
    private readonly IDictionary<string, SKColor> _labelColors;

    public ImageRunner(ILogger<ImageRunner> logger)
    {
        _logger = logger;

        _labelColors = new Dictionary<string, SKColor>();
        var labels = new string[]
        {
            "person", "bicycle", "car", "motorcycle", "airplane", "bus", "train", "truck", "boat", "traffic light",
            "fire hydrant", "stop sign", "parking meter", "bench", "bird", "cat", "dog", "horse", "sheep", "cow",
            "elephant", "bear", "zebra", "giraffe", "backpack", "umbrella", "handbag", "tie", "suitcase", "frisbee",
            "skis", "snowboard", "sports ball", "kite", "baseball bat", "baseball glove", "skateboard", "surfboard",
            "tennis racket", "bottle", "wine glass", "cup", "fork", "knife", "spoon", "bowl", "banana", "apple",
            "sandwich", "orange", "broccoli", "carrot", "hot dog", "pizza", "donut", "cake", "chair", "couch",
            "potted plant", "bed", "dining table", "toilet", "tv", "laptop", "mouse", "remote", "keyboard",
            "cell phone", "microwave", "oven", "toaster", "sink", "refrigerator", "book", "clock", "vase", "scissors",
            "teddy bear", "hair drier", "toothbrush"
        };

        // generate colors for each label
        var random = new Random();
        foreach (var label in labels)
        {
            _labelColors.Add(label, SKColor.FromHsl(random.Next(0, 360), 100, 50));
        }
    }

    [Command("detect", "Recognize objects in an image")]
    public async Task RunAsync([Option("i", "video url")] string videoUrl, CancellationToken token = default)
    {

        _logger.LogInformation("Constructing pipeline");
        // var modelPath = @"F:\repos\ONNX-YOLOv7-Object-Detection\models\yolov7-tiny_256x320.onnx";
        // var modelPath = @"F:\repos\ultralytics\yolov8n.onnx";
        var modelPath = @"F:\repos\ultralytics\runs\detect\train-oneclass\train6\weights\best.onnx";
        var mlContext = new MLContext()
        {
            FallbackToCpu = true
        };
        var modelInput = new ModelInput()
        {
            ImagePath = @"C:\Users\avabi\Pictures\Zrzut ekranu 2022-12-24 132711.png"

        };
        
        var session = new InferenceSession(modelPath, SessionOptions.MakeSessionOptionWithCudaProvider());
        var inputMeta = session.InputMetadata;
        var inputName = inputMeta.Keys.First();

        var outputMeta = session.OutputMetadata;
        var outputNames = outputMeta.Keys.ToArray();
        var outputShapes = outputMeta.Values.Select(x => x.Dimensions).ToArray();
        
        
        var modelWidth = inputMeta[inputName].Dimensions[3];
        var modelHeight = inputMeta[inputName].Dimensions[2];
        // convert image to tensor of shape (1, 3, 640, 640) using EmguCV
        // output is a tensor of shape (1, 84, 8400)
        var mat = CvInvoke.Imread(modelInput.ImagePath);
        var resized = new Mat();
        CvInvoke.Resize(mat, resized, new Size(modelWidth, modelHeight));

        // normalize image


        // convert to BGR
        var converted = new Mat();
        CvInvoke.CvtColor(resized, converted, ColorConversion.Rgb2Bgr);
        // normalize to 0-1 float16
        var normalized = new Mat();
        converted.ConvertTo(normalized, DepthType.Cv32F, 1.0 / 255.0);
        // convert to tensor
        var tensor = normalized.GetData().ToTensor<float>();
        var dense = tensor.Reshape(new[] {1, 3, modelWidth, modelHeight});
        // convert tensor to Half precision
        

        var input = new List<NamedOnnxValue>()
        {
            NamedOnnxValue.CreateFromTensor(inputName, dense)
        };
        
        // run inference
        var output = session.Run(input);
        DisposableNamedOnnxValue outputTensor = output.First();
        
        var outputData = outputTensor.AsTensor<float>();
    }
}

public record ModelInput
{
    [ColumnName("images")]
    public string ImagePath { get; set; }
}

public record ModelOutput
{
    [ColumnName("output")]
    public Tensor Output { get; set; }
}

public record OnnxImagePrediction
{
}

public record ImageDetectionResult(SingleImagePrediction Prediction, SKBitmap Bitmap, string ImagePath);