using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Pickers.Provider;

namespace Xamarin.Media
{
	public class MediaPicker
	{
		public MediaPicker()
		{
			IsCameraAvailable = true; // TODO
		}

		public bool IsCameraAvailable
		{
			get;
			private set;
		}

		public bool PhotosSupported
		{
			get { return true; }
		}

		public bool VideosSupported
		{
			get { return true; }
		}

		public async Task<MediaFile> TakePhotoAsync(StoreCameraMediaOptions options)
		{
			options.VerifyOptions();

			var capture = new CameraCaptureUI();
			var result = await capture.CaptureFileAsync (CameraCaptureUIMode.Photo);
			if (result == null)
				return null;  // TODO - Need to investigate await task cancellation handling

			IStorageFolder folder = KnownFolders.PicturesLibrary;

			string path = options.GetUniqueFilepath (folder.Path, p => StorageFile.GetFileFromPathAsync(p).AsTask().Result == null);
			folder = await StorageFolder.GetFolderFromPathAsync (Path.GetDirectoryName(path));
			string filename = Path.GetFileName (path);

			IStorageFile file = await result.CopyAsync (folder, filename);
			return new MediaFile (file.Path, () => file.OpenStreamForReadAsync().Result);
		}

		public async Task<MediaFile> PickPhotoAsync()
		{
			var picker = new FileOpenPicker();
			picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
			picker.ViewMode = PickerViewMode.Thumbnail;
			picker.FileTypeFilter.Add (".jpg");
			picker.FileTypeFilter.Add (".gif");
			picker.FileTypeFilter.Add (".png");

			var result = await picker.PickSingleFileAsync();
			if (result == null)
				return null; // TODO

			return new MediaFile(result.Path, () => result.OpenStreamForReadAsync().Result);
		}

		public async Task<MediaFile> TakeVideoAsync(StoreVideoOptions options)
		{
			options.VerifyOptions();

			var capture = new CameraCaptureUI();
			capture.VideoSettings.MaxResolution = GetResolutionFromQuality (options.Quality);
			capture.VideoSettings.MaxDurationInSeconds = (float)options.DesiredLength.TotalSeconds;
			capture.VideoSettings.Format = CameraCaptureUIVideoFormat.Mp4;

			var result = await capture.CaptureFileAsync (CameraCaptureUIMode.Video);
			if (result == null)
				return null; // TODO

			return new MediaFile (result.Path, () => result.OpenStreamForReadAsync().Result);
		}

		public async Task<MediaFile> PickVideoAsync()
		{
			var picker = new FileOpenPicker();
			picker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
			picker.ViewMode = PickerViewMode.Thumbnail;
			picker.FileTypeFilter.Add (".mp4");

			var result = await picker.PickSingleFileAsync();
			if (result == null)
				return null; // TODO

			return new MediaFile(result.Path, () => result.OpenStreamForReadAsync().Result);
		}

		private CameraCaptureUIMaxVideoResolution GetResolutionFromQuality(VideoQuality quality)
		{
			switch (quality)
			{
				case VideoQuality.High:
					return CameraCaptureUIMaxVideoResolution.HighDefinition;
				case VideoQuality.Medium:
					return CameraCaptureUIMaxVideoResolution.StandardDefinition;
				case VideoQuality.Low:
					return CameraCaptureUIMaxVideoResolution.LowDefinition;
				default:
					return CameraCaptureUIMaxVideoResolution.HighestAvailable;
			}
		}
	}
}