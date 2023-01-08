using Microsoft.ML.OnnxRuntime;
using SkiaSharp;

namespace ObjectDetector.Yolo.Onnx;

public static class BitmapExtensions
{
    public static OnnxImageData ConvertToData(this SKBitmap img, int modelWidth, int modelHeight)
    {
        var width = img.Width;
        var height = img.Height;
        if (img.Width != modelWidth || img.Height != modelHeight)
        {
            img = img.Resize(new SKImageInfo(modelWidth, modelHeight), SKFilterQuality.High);
        }
        
        // 2. Extract pixels to tensor
        var tensor = Utils.ExtractPixels(img);

        return new OnnxImageData(width, height, NamedOnnxValue.CreateFromTensor("images", tensor));
    }
}