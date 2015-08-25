﻿//
//  Copyright 2011-2013, Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;

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

			this.init = DeviceInformation.FindAllAsync (DeviceClass.VideoCapture).AsTask()
			                             .ContinueWith (t =>
			                             {
				                             if (t.IsFaulted || t.IsCanceled)
					                             return;

				                             lock (this.devices)
				                             {
					                             foreach (DeviceInformation device in t.Result)
					                             {
						                             if (device.IsEnabled)
							                             this.devices.Add (device.Id);
					                             }

					                             this.isCameraAvailable = (this.devices.Count > 0);
				                             }

				                             this.init = null;
			                             });
		}

		public bool IsCameraAvailable
		{
			get
			{
				if (this.init != null)
					this.init.Wait();

				return this.isCameraAvailable;
			}
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
			var result = await capture.CaptureFileAsync (CameraCaptureUIMode.Photo).AsTask (false);
			if (result == null)
				throw new TaskCanceledException();

			StorageFolder folder = ApplicationData.Current.LocalFolder;

			string path = options.GetFilePath (folder.Path);
			folder = await StorageFolder.GetFolderFromPathAsync (Path.GetDirectoryName (path));
			string filename = Path.GetFileName (path);

			IStorageFile file = await result.CopyAsync (folder, filename, NameCollisionOption.GenerateUniqueName).AsTask (false);
			return new MediaFile (file.Path, () => file.OpenStreamForReadAsync().Result);
		}

		public async Task<MediaFile> PickPhotoAsync()
		{
			var picker = new FileOpenPicker();
			picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
			picker.ViewMode = PickerViewMode.Thumbnail;
			picker.FileTypeFilter.Add (".bmp");
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

			var result = await capture.CaptureFileAsync (CameraCaptureUIMode.Video).AsTask (false);
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

			var result = await picker.PickSingleFileAsync().AsTask (false);
			if (result == null)
				throw new TaskCanceledException();

			return new MediaFile (result.Path, () => result.OpenStreamForReadAsync().Result);
		}

		private Task init;
		private readonly HashSet<string> devices = new HashSet<string>();
		private readonly DeviceWatcher watcher;
		private bool isCameraAvailable;

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

				this.isCameraAvailable = this.devices.Count > 0;
			}
		}

		private void OnDeviceRemoved (DeviceWatcher sender, DeviceInformationUpdate update)
		{
			lock (this.devices)
			{
				this.devices.Remove (update.Id);
				if (this.devices.Count == 0)
					this.isCameraAvailable = false;
			}
		}

		private void OnDeviceAdded (DeviceWatcher sender, DeviceInformation device)
		{
			if (!device.IsEnabled)
				return;

			lock (this.devices)
			{
				this.devices.Add (device.Id);
				this.isCameraAvailable = true;
			}
		}
	}
}