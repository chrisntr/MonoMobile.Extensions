using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
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
			this.watcher = DeviceInformation.CreateWatcher (DeviceClass.VideoCapture);
			this.watcher.Added += OnDeviceAdded;
			this.watcher.Updated += OnDeviceUpdated;
			this.watcher.Removed += OnDeviceRemoved;
			this.watcher.Start();
			
			var find = DeviceInformation.FindAllAsync (DeviceClass.VideoCapture);
			find.Completed = (info, status) =>
			{
				if (status != AsyncStatus.Completed)
					return;

				lock (this.devices)
				{
					foreach (DeviceInformation device in info.GetResults())
					{
						if (device.IsEnabled)
							this.devices.Add (device.Id);
					}

					IsCameraAvailable = (this.devices.Count > 0);
				}
			};
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

		public async Task<MediaFile> TakePhotoAsync (StoreCameraMediaOptions options)
		{
			options.VerifyOptions();

			var capture = new CameraCaptureUI();
			var result = await capture.CaptureFileAsync (CameraCaptureUIMode.Photo);
			if (result == null)
				throw new TaskCanceledException();

			IStorageFolder folder = KnownFolders.PicturesLibrary;

			string path = options.GetUniqueFilepath (folder.Path, p => StorageFile.GetFileFromPathAsync(p).AsTask().Result == null);
			folder = await StorageFolder.GetFolderFromPathAsync (Path.GetDirectoryName (path));
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
				throw new TaskCanceledException();

			return new MediaFile (result.Path, () => result.OpenStreamForReadAsync().Result);
		}

		public async Task<MediaFile> TakeVideoAsync (StoreVideoOptions options)
		{
			options.VerifyOptions();

			var capture = new CameraCaptureUI();
			capture.VideoSettings.MaxResolution = GetResolutionFromQuality (options.Quality);
			capture.VideoSettings.MaxDurationInSeconds = (float)options.DesiredLength.TotalSeconds;
			capture.VideoSettings.Format = CameraCaptureUIVideoFormat.Mp4;

			var result = await capture.CaptureFileAsync (CameraCaptureUIMode.Video);
			if (result == null)
				throw new TaskCanceledException();

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
				throw new TaskCanceledException();

			return new MediaFile (result.Path, () => result.OpenStreamForReadAsync().Result);
		}

		private readonly HashSet<string> devices = new HashSet<string>();
		private readonly DeviceWatcher watcher;

		private CameraCaptureUIMaxVideoResolution GetResolutionFromQuality (VideoQuality quality)
		{
			switch (quality)
			{
				case VideoQuality.High:
					return CameraCaptureUIMaxVideoResolution.HighestAvailable;
				case VideoQuality.Medium:
					return CameraCaptureUIMaxVideoResolution.StandardDefinition;
				case VideoQuality.Low:
					return CameraCaptureUIMaxVideoResolution.LowDefinition;
				default:
					return CameraCaptureUIMaxVideoResolution.HighestAvailable;
			}
		}

		private void OnDeviceUpdated (DeviceWatcher sender, DeviceInformationUpdate update)
		{
			object value;
			if (!update.Properties.TryGetValue ("System.Devices.InterfaceEnabled", out value))
				return;

			lock (this.devices)
			{
				if ((bool)value)
					this.devices.Add (update.Id);
				else
					this.devices.Remove (update.Id);

				IsCameraAvailable = this.devices.Count > 0;
			}
		}

		private void OnDeviceRemoved (DeviceWatcher sender, DeviceInformationUpdate update)
		{
			lock (this.devices)
			{
				this.devices.Remove (update.Id);
				if (this.devices.Count == 0)
					IsCameraAvailable = false;
			}
		}

		private void OnDeviceAdded (DeviceWatcher sender, DeviceInformation device)
		{
			if (!device.IsEnabled)
				return;

			lock (this.devices)
			{
				this.devices.Add (device.Id);
				if (!IsCameraAvailable)
					IsCameraAvailable = true;
			}
		}
	}
}