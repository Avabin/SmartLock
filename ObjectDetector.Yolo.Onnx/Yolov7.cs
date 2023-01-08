using System.Collections.Concurrent;
using System.Drawing;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SkiaSharp;
using Yolo.Core;

namespace ObjectDetector.Yolo.Onnx;

public  class Yolov7 : IDisposable
{
    private readonly Lazy<InferenceSession> _inferenceSession;
    public YoloModel Model = new();
    public InferenceSession InferenceSession => _inferenceSession.Value;

    public Yolov7(string ModelPath, bool useCuda = false)
    {

        if (useCuda)
        {
            SessionOptions opts = SessionOptions.MakeSessionOptionWithCudaProvider();
            _inferenceSession = new Lazy<InferenceSession>(() => new InferenceSession(ModelPath, opts));
        }
        else
        {
            SessionOptions opts = new();
            _inferenceSession = new Lazy<InferenceSession>(() => new InferenceSession(ModelPath, opts));
        }


        /// Get model info
        get_input_details();
        get_output_details();
    }

    public void SetupLabels(string[] labels)
    {
        labels.Select((s, i) => new { i, s }).ToList().ForEach(item =>
        {
            Model.Labels.Add(new YoloLabel { Id = item.i, Name = item.s });
        });
    }

    public void SetupYoloDefaultLabels()
    {
        var s = new string[] { "person", "bicycle", "car", "motorcycle", "airplane", "bus", "train", "truck", "boat", "traffic light", "fire hydrant", "stop sign", "parking meter", "bench", "bird", "cat", "dog", "horse", "sheep", "cow", "elephant", "bear", "zebra", "giraffe", "backpack", "umbrella", "handbag", "tie", "suitcase", "frisbee", "skis", "snowboard", "sports ball", "kite", "baseball bat", "baseball glove", "skateboard", "surfboard", "tennis racket", "bottle", "wine glass", "cup", "fork", "knife", "spoon", "bowl", "banana", "apple", "sandwich", "orange", "broccoli", "carrot", "hot dog", "pizza", "donut", "cake", "chair", "couch", "potted plant", "bed", "dining table", "toilet", "tv", "laptop", "mouse", "remote", "keyboard", "cell phone", "microwave", "oven", "toaster", "sink", "refrigerator", "book", "clock", "vase", "scissors", "teddy bear", "hair drier", "toothbrush" };
        SetupLabels(s);
    }

    public List<YoloPrediction> Predict(OnnxImageData image)
    {
        return ParseDetect(Inference(image)[0], image);
    }

    public List<List<YoloPrediction>> Predict(IEnumerable<OnnxImageData> images)
    {
        var onnxImageDatas = images.ToList();
        return onnxImageDatas.Select(Predict).ToList();
    }

    private List<YoloPrediction> ParseDetect(DenseTensor<float> output, OnnxImageData image)
    {
        var result = new ConcurrentBag<YoloPrediction>();

        var (w, h) = (image.Width, image.Height); // image w and h
        var (xGain, yGain) = (Model.Width / (float)w, Model.Height / (float)h); // x, y gains
        var gain = Math.Min(xGain, yGain); // gain = resized / original

        var (xPad, yPad) = ((Model.Width - w * gain) / 2, (Model.Height - h * gain) / 2); // left, right pads

        Parallel.For(0, output.Dimensions[0], (int i) => {
            var label = Model.Labels[(int)output[i,5]];
            var prediction = new YoloPrediction(label, output[0,6]);

            var xMin = (output[i, 1] - xPad) / gain;
            var yMin = (output[i, 2] - yPad) / gain;
            var xMax = (output[i, 3] - xPad) / gain;
            var yMax = (output[i, 4] - yPad) / gain;

            //install package TensorFlow.Net,SciSharp.TensorFlow.Redist 安装这两个包可以用numpy 进行计算
            //var box = np.array(item.GetValue(1), item.GetValue(2), item.GetValue(3), item.GetValue(4));
            //var tmp =  np.array(xPad, yPad,xPad, yPad) ;
            //box -= tmp;
            //box /= gain;

            var predictionRectangle = new RectangleF(xMin, yMin, xMax - xMin, yMax - yMin);
            
            prediction.Rectangle = SKRect.Create(predictionRectangle.X, predictionRectangle.Y, predictionRectangle.Width, predictionRectangle.Height);
            
            result.Add(prediction);
        });

        return result.ToList();
    }

    

    private DenseTensor<float>[] Inference(IReadOnlyCollection<OnnxImageData> inputs)
    {
        IDisposableReadOnlyCollection<DisposableNamedOnnxValue> result = InferenceSession.Run(inputs.Select(x => x.Data).ToArray()); // run inference

        return Model.Outputs.Select(item => (DenseTensor<float>) result.First(x => x.Name == item).Value).ToArray();
    }
    private DenseTensor<float>[] Inference(OnnxImageData input)
    { // run inference
        IDisposableReadOnlyCollection<DisposableNamedOnnxValue> result = InferenceSession.Run(new List<NamedOnnxValue> {input.Data}); // run inference

        return Model.Outputs.Select(item => (DenseTensor<float>) result.First(x => x.Name == item).Value).ToArray();
    }

    private void get_input_details()
    {
        Model.Height = InferenceSession.InputMetadata["images"].Dimensions[2];
        Model.Width = InferenceSession.InputMetadata["images"].Dimensions[3];
    }

    private void get_output_details()
    {
        Model.Outputs = InferenceSession.OutputMetadata.Keys.ToArray();
        Model.Dimensions = InferenceSession.OutputMetadata[Model.Outputs[0]].Dimensions[1];
        Model.UseDetect = !(Model.Outputs.Any(x => x == "score"));
    }

    public void Dispose()
    {
        InferenceSession.Dispose();
    }
}