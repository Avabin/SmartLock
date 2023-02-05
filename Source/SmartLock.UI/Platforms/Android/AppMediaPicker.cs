namespace SmartLock.UI;

public partial class AppMediaPicker
{
    public partial Task<FileResult> CapturePhotoAsync(MediaPickerOptions? options)
        => MediaPicker.Default.CapturePhotoAsync(options);

    public partial Task<FileResult> CaptureVideoAsync(MediaPickerOptions? options)
        => MediaPicker.Default.CaptureVideoAsync(options);
}