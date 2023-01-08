using SkiaSharp;

namespace ObjectDetector;

public record SingleImagePrediction(TimeSpan Elapsed, List<DetectionResult> Predictions)
{
    public SKBitmap DrawBoundingBoxes(SKBitmap image, IDictionary<string, SKColor> colors, float threshold = 0.5f)
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
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Red,
            StrokeWidth = 1
        };

        foreach (var (rect, label, confidence) in from prediction in Predictions where !(prediction.Confidence < threshold) select (prediction.BoundingBox, prediction.Label, prediction.Confidence))
        {
            var textPaint = new SKPaint
            {
                Color = colors[label],
                TextSize = 20,
                IsAntialias = true,
                IsStroke = false
            };
            canvas.DrawRect(rect, paint);
            var labelText = $"{label}: {confidence:P2}";
            // check if text fits on the top of the bounding box
            if (rect.Top - textPaint.TextSize > 0)
            {
                canvas.DrawText(labelText, rect.Left, rect.Top - textPaint.TextSize, textPaint);
            }
            else
            {
                canvas.DrawText(labelText, rect.Left, rect.Bottom + textPaint.TextSize, textPaint);
            }
        }

        return image;
    }
}