using Microsoft.ML.OnnxRuntime.Tensors;
using SkiaSharp;

namespace Yolov7.Yolov7;

public static class Utils
{
    /// <summary>
    /// xywh to xyxy
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static float[] Xywh2xyxy(float[] source)
    {
        var result = new float[4];

        result[0] = source[0] - source[2] / 2f;
        result[1] = source[1] - source[3] / 2f;
        result[2] = source[0] + source[2] / 2f;
        result[3] = source[1] + source[3] / 2f;

        return result;
    }

    public static SKBitmap ResizeImage(SKBitmap image,int target_width,int target_height)
    {
        // scale the image to the new size
        var scaled = new SKBitmap(target_width, target_height);
        image.ScalePixels(scaled, SKFilterQuality.High);
        return scaled;
    }

    public static Tensor<float> ExtractPixels(SKBitmap image)
    {
        var tensor = new DenseTensor<float>(new[] { 1, 3, image.Height, image.Width });
        var pixels = image.Pixels;
        var width = image.Width;
        var height = image.Height;
        var index = 0;
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pixel = pixels[index];
                tensor[0, 0, y, x] = ((pixel.Red & 0x00FF0000) >> 16) / 255f;
                tensor[0, 1, y, x] = ((pixel.Green & 0x0000FF00) >> 8) / 255f;
                tensor[0, 2, y, x] = (pixel.Blue & 0x000000FF) / 255f;
                index++;
            }
        }

        return tensor;
    }

    public static float Clamp(float value, float min, float max) =>
        (value < min) 
            ? min 
            : (value > max) 
                ? max 
                : value;
}