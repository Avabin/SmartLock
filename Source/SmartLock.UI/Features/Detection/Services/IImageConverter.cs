using IImage = Microsoft.Maui.Graphics.IImage;

namespace SmartLock.UI.Features.Detection.Services;

public interface IImageConverter
{
    Task<IImage> ToImage(FileResult fileResult);
}