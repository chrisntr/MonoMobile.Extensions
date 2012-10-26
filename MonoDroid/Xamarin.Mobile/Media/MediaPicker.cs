using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;

namespace Xamarin.Media
{
	public class MediaPicker
	{
		public MediaPicker (Context context)
		{
			this.context = context;
			IsCameraAvailable = context.PackageManager.HasSystemFeature (PackageManager.FeatureCamera);

			if (Build.VERSION.SdkInt >= BuildVersionCodes.Gingerbread)
				IsCameraAvailable |= context.PackageManager.HasSystemFeature (PackageManager.FeatureCameraFront);
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

		public Task<MediaFile> PickPhotoAsync()
		{
			return TakeMediaAsync ("image/*", Intent.ActionPick, null);
		}

		public Task<MediaFile> TakePhotoAsync (StoreCameraMediaOptions options)
		{
			if (!IsCameraAvailable)
				throw new NotSupportedException();

			VerifyOptions (options);

			return TakeMediaAsync ("image/*", MediaStore.ActionImageCapture, options);
		}

		public Task<MediaFile> PickVideoAsync()
		{
			return TakeMediaAsync ("video/*", Intent.ActionPick, null);
		}

		public Task<MediaFile> TakeVideoAsync (StoreVideoOptions options)
		{
			if (!IsCameraAvailable)
				throw new NotSupportedException();

			VerifyOptions (options);

			return TakeMediaAsync ("video/*", MediaStore.ActionVideoCapture, options);
		}

		private readonly Context context;
		private int requestId;
		private TaskCompletionSource<MediaFile> completionSource;

		private void VerifyOptions (StoreMediaOptions options)
		{
			if (options == null)
				throw new ArgumentNullException ("options");
			//if (!Enum.IsDefined (typeof(MediaFileStoreLocation), options.Location))
			//    throw new ArgumentException ("options.Location is not a member of MediaFileStoreLocation");
			//if (options.Location == MediaFileStoreLocation.Local)
			//{
				//if (String.IsNullOrWhiteSpace (options.Directory))
				//	throw new ArgumentNullException ("options", "For local storage, options.Directory must be set");
				if (Path.IsPathRooted (options.Directory))
					throw new ArgumentException ("options.Directory must be a relative path", "options");
			//}
		}

		private Task<MediaFile> TakeMediaAsync (string type, string action, StoreMediaOptions options)
		{
			int id = this.requestId;
			if (this.requestId == Int32.MaxValue)
				this.requestId = 0;
			else
				this.requestId++;

			var ntcs = new TaskCompletionSource<MediaFile> (id);
			if (Interlocked.CompareExchange (ref this.completionSource, ntcs, null) != null)
				throw new InvalidOperationException ("Only one operation can be active at a time");

			Intent pickerIntent = new Intent (this.context, typeof (MediaPickerActivity));
			pickerIntent.PutExtra (MediaPickerActivity.ExtraType, type);
			pickerIntent.PutExtra (MediaPickerActivity.ExtraId, id);
			pickerIntent.PutExtra (MediaPickerActivity.ExtraAction, action);

			if (options != null)
			{
				//pickerIntent.PutExtra (MediaPickerActivity.ExtraLocation, (int)options.Location);
				pickerIntent.PutExtra (MediaPickerActivity.ExtraPath, options.Directory);
				pickerIntent.PutExtra (MediaStore.Images.ImageColumns.Title, options.Name);
				//if (options.Description != null)
				//    pickerIntent.PutExtra (MediaStore.Images.ImageColumns.Description, options.Description);
			}

			var vidOptions = (options as StoreVideoOptions);
			if (vidOptions != null)
			{
				pickerIntent.PutExtra (MediaStore.ExtraDurationLimit, (int)vidOptions.DesiredLength.TotalSeconds);
				pickerIntent.PutExtra (MediaStore.ExtraVideoQuality, (int)vidOptions.Quality);
			}

			this.context.StartActivity (pickerIntent);

			EventHandler<MediaPickedEventArgs> handler = null;
			handler = (s, e) =>
			{
				TaskCompletionSource<MediaFile> tcs = Interlocked.Exchange (ref this.completionSource, null);

				MediaPickerActivity.MediaPicked -= handler;

				if (e.RequestId != id)
					return;

				if (e.Error != null)
					tcs.SetException (e.Error);
				else if (e.IsCanceled)
					tcs.SetCanceled();
				else
					tcs.SetResult (e.Media);
			};

			MediaPickerActivity.MediaPicked += handler;

			return ntcs.Task;
		}
	}
}