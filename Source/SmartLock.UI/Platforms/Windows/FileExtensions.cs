using Windows.Storage;

namespace SmartLock.UI;

internal static class FileExtensions
{
    internal static FileResult ToFileResult(this StorageFile file)
    {
        var fullPath = file.Path;
        var contentType = file.ContentType;
        return new FileResult(new ReadOnlyFile(fullPath, contentType));
    }
}