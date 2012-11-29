using System;
using System.Threading.Tasks;
using MonoTouch.UIKit;
using System.Threading;
using System.IO;
using System.Linq;
using System.Drawing;

namespace Xamarin.Media
{
	public class MediaPicker
	{
		public MediaPicker ()
		{
			IsCameraAvailable = UIImagePickerController.IsSourceTypeAvailable (UIImagePickerControllerSourceType.Camera);

			string[] availableCameraMedia = UIImagePickerController.AvailableMediaTypes (UIImagePickerControllerSourceType.Camera) ?? new string[0];
			string[] avaialbleLibraryMedia = UIImagePickerController.AvailableMediaTypes (UIImagePickerControllerSourceType.PhotoLibrary) ?? new string[0];

			foreach (string type in availableCameraMedia.Concat (avaialbleLibraryMedia))
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
			if (!PhotosSupported)
				throw new NotSupportedException();
			
			return TakeMedia (UIImagePickerControllerSourceType.PhotoLibrary, TypeImage);
		}

		public Task<MediaFile> TakePhotoAsync (StoreCameraMediaOptions options)
		{
			if (!PhotosSupported)
				throw new NotSupportedException();
			if (!IsCameraAvailable)
				throw new NotSupportedException();
			
			VerifyCameraOptions (options);

			return TakeMedia (UIImagePickerControllerSourceType.Camera, TypeImage, options);
		}

		public Task<MediaFile> PickVideoAsync()
		{
			if (!VideosSupported)
				throw new NotSupportedException();
			
			return TakeMedia (UIImagePickerControllerSourceType.PhotoLibrary, TypeMovie);
		}

		public Task<MediaFile> TakeVideoAsync (StoreVideoOptions options)
		{
			if (!VideosSupported)
				throw new NotSupportedException();
			if (!IsCameraAvailable)
				throw new NotSupportedException();
			
			VerifyCameraOptions (options);

			return TakeMedia (UIImagePickerControllerSourceType.Camera, TypeMovie, options);
		}

		private UIPopoverController popover;
		private UIImagePickerControllerDelegate pickerDelegate;
		internal const string TypeImage = "public.image";
		internal const string TypeMovie = "public.movie";

		private void VerifyOptions (StoreMediaOptions options)
		{
			if (options == null)
				throw new ArgumentNullException ("options");
			//if (!Enum.IsDefined (typeof(MediaFileStoreLocation), options.Location))
			//	throw new ArgumentException ("options.Location is not a member of MediaFileStoreLocation");
			//if (options.Location == MediaFileStoreLocation.Local)
			//{
				if (options.Directory != null && Path.IsPathRooted (options.Directory))
					throw new ArgumentException ("options.Directory must be a relative path", "options");
			//}
		}

		private void VerifyCameraOptions (StoreCameraMediaOptions options)
		{
			VerifyOptions (options);
			if (!Enum.IsDefined (typeof(CameraDevice), options.DefaultCamera))
				throw new ArgumentException ("options.Camera is not a member of CameraDevice");
		}

		private Task<MediaFile> TakeMedia (UIImagePickerControllerSourceType sourceType, string mediaType, StoreCameraMediaOptions options = null)
		{
			UIWindow window = UIApplication.SharedApplication.KeyWindow;
			if (window == null)
				throw new InvalidOperationException ("There's no current active window");

			UIViewController viewController = window.RootViewController;

			if (viewController == null) {
				window = UIApplication.SharedApplication.Windows.OrderByDescending (w => w.WindowLevel).FirstOrDefault (w => w.RootViewController != null);
				if (window == null)
					throw new InvalidOperationException ("Could not find current view controller");
				else
					viewController = window.RootViewController;	
			}

			while (viewController.PresentedViewController != null)
				viewController = viewController.PresentedViewController;

			MediaPickerDelegate ndelegate = new MediaPickerDelegate (viewController, sourceType, options);
			var od = Interlocked.CompareExchange (ref this.pickerDelegate, ndelegate, null);
			if (od != null)
				throw new InvalidOperationException ("Only one operation can be active at at time");

			var picker = new UIImagePickerController();
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

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			{	
				ndelegate.Popover = new UIPopoverController (picker);
				ndelegate.Popover.Delegate = new MediaPickerPopoverDelegate (ndelegate, picker);
				ndelegate.DisplayPopover();
			}
			else
				viewController.PresentModalViewController (picker, true);

			return ndelegate.Task
				.ContinueWith (t =>
				{
					if (this.popover != null)
					{
						this.popover.Dispose();
						this.popover = null;
					}
					
					Interlocked.Exchange (ref this.pickerDelegate, null);
					return t;
				}).Unwrap();
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