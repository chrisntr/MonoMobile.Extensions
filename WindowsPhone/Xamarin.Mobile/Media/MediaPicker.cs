using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Devices;
using Microsoft.Phone.Tasks;

namespace Xamarin.Media
{
	public class MediaPicker
	{
		public MediaPicker()
		{
			this.photoChooser.Completed += OnPhotoChosen;
			this.photoChooser.ShowCamera = false;

			this.cameraCapture.Completed += OnPhotoChosen;

			IsCameraAvailable = Camera.IsCameraTypeSupported (CameraType.Primary) || Camera.IsCameraTypeSupported (CameraType.FrontFacing);
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
			get { return false; }
		}

		public Task<MediaFile> PickPhotoAsync()
		{
			var ntcs = new TaskCompletionSource<MediaFile>();
			if (Interlocked.CompareExchange (ref this.completionSource, ntcs, null) != null)
				throw new InvalidOperationException ("Only one operation can be active at at time");

			this.photoChooser.Show();

			return ntcs.Task;
		}

		public Task<MediaFile> TakePhotoAsync (StoreCameraMediaOptions options)
		{
			options.VerifyOptions();

			var ntcs = new TaskCompletionSource<MediaFile> (options);
			if (Interlocked.CompareExchange (ref this.completionSource, ntcs, null) != null)
				throw new InvalidOperationException ("Only one operation can be active at a time");

			this.cameraCapture.Show();

			return ntcs.Task;
		}

		public Task<MediaFile> PickVideoAsync()
		{
			throw new NotSupportedException();
		}

		public Task<MediaFile> TakeVideoAsync (StoreVideoOptions options)
		{
			throw new NotSupportedException();
		}

		private readonly CameraCaptureTask cameraCapture = new CameraCaptureTask();
		private readonly PhotoChooserTask photoChooser = new PhotoChooserTask();
		private TaskCompletionSource<MediaFile> completionSource;

		private void OnPhotoChosen (object sender, PhotoResult photoResult)
		{
			var tcs = Interlocked.Exchange (ref this.completionSource, null);

			string path = photoResult.OriginalFileName;

			var options = tcs.Task.AsyncState as StoreCameraMediaOptions;
			if (options != null)
			{
				using (var store = IsolatedStorageFile.GetUserStoreForApplication())
				{
					path = options.GetUniqueFilepath (null, p => store.FileExists (p));

					using (var fs = store.CreateFile (path))
					using (var s = photoResult.ChosenPhoto)
					{
						byte[] buffer = new byte[20480];
						int len;
						while ((len = s.Read (buffer, 0, buffer.Length)) > 0)
							fs.Write (buffer, 0, len);

						fs.Flush (true);
					}
				}
			}

			switch (photoResult.TaskResult)
			{
				case TaskResult.OK:
					tcs.SetResult (new MediaFile (path));
					break;

				case TaskResult.None:
					if (photoResult.Error != null)
						tcs.SetException (photoResult.Error);

					break;

				case TaskResult.Cancel:
					tcs.SetCanceled();
					break;
			}
		}
	}
}