using Microsoft.Maui.Graphics.Skia;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace SmartLock.UI.Features.Detection.Services;

public static class ImageConverter
{
    public static async Task<IImage> ToImage(FileResult fileResult) => 
        SkiaImage.FromStream(await fileResult.OpenReadAsync());
    
    public static IImage ToImage(Stream stream) => SkiaImage.FromStream(stream);
}