using System.Reflection;

namespace SmartLock.UI;

using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.System;
using WinRT.Interop;

public partial class AppMediaPicker
{
	public partial Task<FileResult> CapturePhotoAsync(MediaPickerOptions? options)
		=> CaptureAsync(false);

	public partial Task<FileResult> CaptureVideoAsync(MediaPickerOptions? options)
		=> CaptureAsync(true);

	private async Task<FileResult> CaptureAsync(bool isVideo)
	{
		var captureUi = new CustomCameraCaptureUi();

		var file = await captureUi.CaptureFileAsync(isVideo ? CameraCaptureUIMode.Video : CameraCaptureUIMode.Photo);

		if (file != null)
		{
			// to mitigate existing issue with WinUI and MediaPicker
			// we need to create a FileResult instance manually
			// and return it instead of using MediaPicker
			// see https://github.com/microsoft/WindowsAppSDK/issues/1034
			// TODO: remove this workaround when the issue is fixed
			
			
			// find FileResult internal constructor with a IStorageFile parameter
			var fileResultConstructor = typeof(FileResult).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).First(x => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(IStorageFile));
			
			// create FileResult instance
			var instance = (FileResult)fileResultConstructor.Invoke(new object[] { file });
			
			// return FileResult instance
			return instance;
		}

		throw new InvalidOperationException("Failed to capture file.");
	}

	private class CustomCameraCaptureUi
	{
		private readonly LauncherOptions _launcherOptions;

		public CustomCameraCaptureUi()
		{
			var window = WindowStateManager.Default.GetActiveWindow();
			var handle = WindowNative.GetWindowHandle(window);

			_launcherOptions = new LauncherOptions();
			InitializeWithWindow.Initialize(_launcherOptions, handle);

			_launcherOptions.TreatAsUntrusted = false;
			_launcherOptions.DisplayApplicationPicker = false;
			_launcherOptions.TargetApplicationPackageFamilyName = "Microsoft.WindowsCamera_8wekyb3d8bbwe";
		}

		public async Task<StorageFile> CaptureFileAsync(CameraCaptureUIMode mode)
		{
			var extension = mode == CameraCaptureUIMode.Photo ? ".jpg" : ".mp4";

			var currentAppData = ApplicationData.Current;
			var tempLocation = currentAppData.LocalCacheFolder;
			var tempFileName = $"capture{extension}";
			var tempFile = await tempLocation.CreateFileAsync(tempFileName, CreationCollisionOption.GenerateUniqueName);
			var token = Windows.ApplicationModel.DataTransfer.SharedStorageAccessManager.AddFile(tempFile);

			var set = new ValueSet();
			if (mode == CameraCaptureUIMode.Photo)
			{
				set.Add("MediaType", "photo");
				set.Add("PhotoFileToken", token);
			}
			else
			{
				set.Add("MediaType", "video");
				set.Add("VideoFileToken", token);
			}

			var uri = new Uri("microsoft.windows.camera.picker:");
			var result = await Windows.System.Launcher.LaunchUriForResultsAsync(uri, _launcherOptions, set);
			if (result.Status == LaunchUriStatus.Success && result.Result != null)
			{
				return tempFile;
			}

			throw new InvalidOperationException("Failed to capture file.");
		}
	}
}