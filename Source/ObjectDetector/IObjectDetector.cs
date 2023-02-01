using Yolo.Core;

namespace ObjectDetector;

public interface IObjectDetector<in T> : IObjectDetector where T : IImageData
{
    Task<SingleImagePrediction> DetectAsync(T image);
    Task<MultipleImagePrediction> DetectAsync(IEnumerable<T> images);
}

public interface IObjectDetector
{
    Task<SingleImagePrediction> DetectAsync(IImageData image);
    Task<MultipleImagePrediction> DetectAsync(IEnumerable<IImageData> images);
    
    (int Width, int Height) GetInputSize();
    IReadOnlyCollection<string> GetLabels();
}