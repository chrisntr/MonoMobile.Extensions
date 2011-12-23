using System;
using System.Threading.Tasks;
using MonoTouch.UIKit;
using System.Threading;
using System.IO;
using System.Linq;

namespace Xamarin.Media
{
	public class MediaPicker
	{
		public MediaPicker ()
		{
			IsCameraAvailable = UIImagePickerController.IsSourceTypeAvailable (UIImagePickerControllerSourceType.Camera);

			string[] mediaTypes = UIImagePickerController.AvailableMediaTypes (UIImagePickerControllerSourceType.Camera)
									.Intersect (UIImagePickerController.AvailableMediaTypes (UIImagePickerControllerSourceType.PhotoLibrary))
				            		.ToArray();

			foreach (string type in mediaTypes)
			{
				if (type == TypeMovie)
					VideosSupported = true;
				else if (type == TypeImage)
					PhotosSupported = true;
			}
		}

		public bool IsCameraAvailable
		{
			get;
			private set;
		}

		public bool PhotosSupported
		{
			get;
			private set;
		}

		public bool VideosSupported
		{
			get;
			private set;
		}

		public Task<MediaFile> PickPhotoAsync()
		{
			return TakeMedia (UIImagePickerControllerSourceType.PhotoLibrary, TypeImage);
		}

		public Task<MediaFile> TakePhotoAsync (StoreCameraMediaOptions options)
		{
			VerifyCameraOptions (options);

			return TakeMedia (UIImagePickerControllerSourceType.Camera, TypeImage, options);
		}

		public Task<MediaFile> PickVideoAsync()
		{
			return TakeMedia (UIImagePickerControllerSourceType.PhotoLibrary, TypeMovie);
		}

		public Task<MediaFile> TakeVideoAsync (StoreVideoOptions options)
		{
			VerifyCameraOptions (options);

			return TakeMedia (UIImagePickerControllerSourceType.Camera, TypeMovie, options);
		}

		private UIImagePickerControllerDelegate pickerDelegate;
		internal const string TypeImage = "public.image";
		internal const string TypeMovie = "public.movie";

		private void VerifyOptions (StoreMediaOptions options)
		{
			if (options == null)
				throw new ArgumentNullException ("options");
			if (!Enum.IsDefined (typeof(MediaFileStoreLocation), options.Location))
				throw new ArgumentException ("options.Location is not a member of MediaFileStoreLocation");
			if (options.Location == MediaFileStoreLocation.Local)
			{
				if (Path.IsPathRooted (options.Directory))
					throw new ArgumentException ("options.Directory must be a relative path", "options");
			}
		}

		private void VerifyCameraOptions (StoreCameraMediaOptions options)
		{
			VerifyOptions (options);
			if (!Enum.IsDefined (typeof(CameraDevice), options.DefaultCamera))
				throw new ArgumentException ("options.Camera is not a member of CameraDevice");
		}

		private Task<MediaFile> TakeMedia (UIImagePickerControllerSourceType sourceType, string mediaType, StoreCameraMediaOptions options = null)
		{
			MediaPickerDelegate ndelegate = new MediaPickerDelegate (options);
			var od = Interlocked.CompareExchange (ref this.pickerDelegate, ndelegate, null);
			if (od != null)
				throw new InvalidOperationException ("Only one operation can be active at at time");

			UIImagePickerController picker = new UIImagePickerController();
			picker.Delegate = ndelegate;
			picker.MediaTypes = new[] { mediaType };
			picker.SourceType = sourceType;

			if (sourceType == UIImagePickerControllerSourceType.Camera)
			{
				picker.CameraDevice = GetUICameraDevice (options.DefaultCamera);
				
				if (mediaType == TypeImage)
				{
					picker.CameraCaptureMode = UIImagePickerControllerCameraCaptureMode.Photo;
				}
				else if (mediaType == TypeMovie)
				{
					StoreVideoOptions voptions = (StoreVideoOptions)options;
					
					picker.CameraCaptureMode = UIImagePickerControllerCameraCaptureMode.Video;
					picker.VideoQuality = GetQuailty (voptions.Quality);
					picker.VideoMaximumDuration = voptions.DesiredLength.TotalSeconds;
				}
			}

			UIWindow window = UIApplication.SharedApplication.KeyWindow;
			UIViewController rootController = window.RootViewController;
			if (rootController != null)
				rootController.PresentModalViewController (picker, true);
			else
			{
				throw new NotImplementedException();
			}

			return ndelegate.Task;
		}
		
		private UIImagePickerControllerCameraDevice GetUICameraDevice (CameraDevice device)
		{
			switch (device)
			{
				case CameraDevice.Front:
					return UIImagePickerControllerCameraDevice.Front;
				case CameraDevice.Rear:
					return UIImagePickerControllerCameraDevice.Rear;
				default:
					throw new NotSupportedException();
			}
		}

		private UIImagePickerControllerQualityType GetQuailty (VideoQuality quality)
		{
			switch (quality)
			{
				case VideoQuality.Low:
					return UIImagePickerControllerQualityType.Low;
				case VideoQuality.Medium:
					return UIImagePickerControllerQualityType.Medium;
				default:
					return UIImagePickerControllerQualityType.High;
			}
		}
	}
}