using System;
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

		public Task<MediaFile> TakePhotoAsync (StoreMediaOptions options)
		{
			if (options == null)
				throw new ArgumentNullException ("options");

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

			var options = tcs.Task.AsyncState as StoreMediaOptions;
			if (options != null)
			{
				switch (options.Location)
				{
					case MediaFileStoreLocation.CameraRoll:
					case MediaFileStoreLocation.Local:
						throw new NotImplementedException();
				}
			}

			switch (photoResult.TaskResult)
			{
				case TaskResult.OK:
					tcs.SetResult (new MediaFile (() => photoResult.ChosenPhoto, null));
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