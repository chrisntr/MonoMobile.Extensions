using System;
using System.Threading.Tasks;
using MonoTouch.UIKit;
using System.Threading;
using System.IO;

namespace Xamarin.Media
{
	public class MediaPicker
	{
		public MediaPicker ()
		{
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
		
		public Task<MediaFile> TakePhotoAsync (StoreMediaOptions options)
		{
			VerifyOptions (options);

			return TakeMedia (UIImagePickerControllerSourceType.Camera, TypeImage, options);
		}
		
		public Task<MediaFile> PickVideoAsync()
		{
			return TakeMedia (UIImagePickerControllerSourceType.PhotoLibrary, TypeVideo);
		}
		
		private UIImagePickerControllerDelegate pickerDelegate;
		internal const string TypeImage = "public.image";
		internal const string TypeVideo = "public.video";
		
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
		
		private Task<MediaFile> TakeMedia (UIImagePickerControllerSourceType sourceType, string mediaType, StoreMediaOptions options = null)
		{
			var ndelegate = new MediaPickerDelegate (options);
			var od = Interlocked.CompareExchange (ref this.pickerDelegate, ndelegate, null);
			if (od != null)
				throw new InvalidOperationException ("Only one operation can be active at at time");
			
			UIImagePickerController picker = new UIImagePickerController();
			picker.Delegate = ndelegate;
				
			picker.SourceType = sourceType;
			picker.MediaTypes = new[] { mediaType };

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
	}
}