using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;
using SmartLock.Client.HttpClient;
using SmartLock.Client.Models;

namespace SmartLock.UI.Features.Detection.ViewModels;

public partial class DetectionViewModel : ObservableObject
{
    private readonly IDetectionService _detectionService;
    private readonly IMediaPicker _mediaPicker;
    
    [field: ObservableProperty] public ImageSource? DetectionImage { get; set; }

    public DetectionViewModel(IDetectionService detectionService, IMediaPicker mediaPicker)
    {
        _detectionService = detectionService;
        _mediaPicker = mediaPicker;
    }
    
    [RelayCommand]
    private async Task DetectFileAsync() => await DetectAsync(true);
    
    [RelayCommand]
    private async Task DetectCameraAsync() => await DetectAsync();
    private async Task DetectAsync(bool isFile = false)
    {
        var photo = await CapturePhotoAsync(isFile);
        
        if (photo is null)
        {
            return;
        }

        var results = await _detectionService.DetectAsync(await photo.OpenReadAsync());

        DetectionImage = await DrawResults(photo, results);
    }

    private async Task<FileResult?> CapturePhotoAsync(bool isFile) =>
        !isFile
            ? await _mediaPicker.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = "Take a photo"
            })
            : await _mediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Pick a photo"
            });

    private static async Task<ImageSource> DrawResults(FileBase photo, IEnumerable<DetectedObjectModel> results)
    {
        var bitmap = SKBitmap.Decode(await photo.OpenReadAsync());
        var surface = SKSurface.Create(new SKImageInfo(bitmap.Width, bitmap.Height));
        surface.Canvas.DrawBitmap(bitmap, 0, 0);
        var skiaCanvas = surface.Canvas;
        
        foreach (var (@class, confidence, x1, y1, x2, y2) in results)
            DrawResult(x1, y1, x2, y2, @class, skiaCanvas, confidence);

        var skiaImage = surface.Snapshot();
        return ImageSource.FromStream(() => skiaImage.Encode(SKEncodedImageFormat.Png, 100).AsStream());
    }

    private static void DrawResult(long x1, long y1, long x2, long y2, string @class, SKCanvas skiaCanvas,
        double confidence)
    {
        // draw bounding box using x1, y1, x2, y2
        var rect = new SKRect(x1, y1, x2, y2);
        // generate color based on pseudorandom generated hash of class name
        var hashCode = @class.GetHashCode();
        // generate color based on pseudorandom hash of class name
        var color = new SKColor((byte)(hashCode >> 16), (byte)(hashCode >> 8), (byte)hashCode);
        var paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = color,
            StrokeWidth = 2
        };
        skiaCanvas.DrawRect(rect, paint);

        // draw label
        var text = $"{@class} ({confidence * 100:0.00}%)";
        var textPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = color,
            TextSize = 35,
            IsAntialias = true
        };
        var textBounds = new SKRect();

        // check if text fits just above the bounding box
        textPaint.MeasureText(text, ref textBounds);
        // scale text size to fit in bounding box to a minimum of 10
        while (textBounds.Width > x2 - x1 && textPaint.TextSize > 10)
        {
            textPaint.TextSize--;
            textPaint.MeasureText(text, ref textBounds);
        }
        var fitsAbove = y1 - textBounds.Height > 0;
        // check if text fits just below the bounding box
        var fitsBelow = y2 + textBounds.Height < skiaCanvas.DeviceClipBounds.Height;

        // if text fits just above the bounding box, draw it there
        if (fitsAbove)
        {
            var textX = x1;
            var textY = y1 - textBounds.Height;
            skiaCanvas.DrawText(text, textX, textY, textPaint);
        }
        // if text fits just below the bounding box, draw it there
        else if (fitsBelow)
        {
            var textX = x1;
            var textY = y2 + textBounds.Height;
            skiaCanvas.DrawText(text, textX, textY, textPaint);
        }
        // if text doesn't fit above or below the bounding box, draw it in the top left corner of the box
        else
        {
            var textX = x1;
            var textY = y1;
            skiaCanvas.DrawText(text, textX, textY, textPaint);
        }
    }
}