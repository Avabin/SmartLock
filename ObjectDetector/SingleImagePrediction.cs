using SkiaSharp;

namespace ObjectDetector;

public record SingleImagePrediction(TimeSpan Elapsed, List<DetectionResult> Predictions)
{
    public SKBitmap DrawBoundingBoxes(SKBitmap image, float threshold = 0.5f, IDictionary<string, SKColor> colors)
    {
        var canvas = new SKCanvas(image);
        var paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Red,
            StrokeWidth = 1
        };

        foreach (var rect in from prediction in Predictions where !(prediction.Confidence < threshold) select prediction.BoundingBox)
        {
            canvas.DrawRect(rect, paint);
        }

        return image;
    }
    
    public SKBitmap DrawPrediction(SKBitmap image, IDictionary<string, SKColor> colors, float threshold = 0.5f)
    {
        var canvas = new SKCanvas(image);
        var paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Red,
            StrokeWidth = 1
        };

        foreach (var (rect, label, confidence) in from prediction in Predictions where !(prediction.Confidence < threshold) select (prediction.BoundingBox, prediction.Label, prediction.Confidence))
        {
            canvas.DrawRect(rect, paint);
            var labelText = $"{label}: {confidence:P2}";
            // if text does not fit, resize the image
            if (rect.Width < paint.MeasureText(labelText))
            {
                var scale = rect.Width / paint.MeasureText(labelText);
                paint.TextSize *= scale;
            }
            
            canvas.DrawText(labelText, rect.Left, rect.Top, paint);
        }

        return image;
    }
}