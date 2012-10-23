using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MonoTouch.AssetsLibrary;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace Xamarin.Media
{
	internal class MediaPickerDelegate
		: UIImagePickerControllerDelegate
	{
		internal MediaPickerDelegate (UIViewController viewController, UIImagePickerControllerSourceType sourceType, StoreCameraMediaOptions options)
		{
			this.viewController = viewController;
			this.source = sourceType;
			this.options = options ?? new StoreCameraMediaOptions();

			UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
			this.observer = NSNotificationCenter.DefaultCenter.AddObserver (UIDevice.OrientationDidChangeNotification, DidRotate);
		}
		
		public UIPopoverController Popover
		{
			get;
			set;
		}
		
		public UIView View
		{
			get { return this.viewController.View; }
		}

		public Task<MediaFile> Task
		{
			get { return tcs.Task; }
		}

		public override void FinishedPickingMedia (UIImagePickerController picker, NSDictionary info)
		{
			Task<NSUrl> saved = null;
			MediaFile mediaFile = null;
			switch ((NSString)info[UIImagePickerController.MediaType])
			{
				case MediaPicker.TypeImage:
					mediaFile = GetPictureMediaFile (info, out saved);
					break;

				case MediaPicker.TypeMovie:
					mediaFile = GetMovieMediaFile (info, out saved);
					break;

				default:
					throw new NotSupportedException();
			}

			//if (saved == null)
			//{
				this.tcs.TrySetResult (mediaFile);
			//}
//			else
//			{
//				saved.ContinueWith (t =>
//				{
//					if (t.IsCanceled)
//						this.tcs.TrySetCanceled();
//					else if (t.IsFaulted)
//						this.tcs.TrySetException (t.Exception);
//					else
//						this.tcs.TrySetResult (mediaFile);
//					
//					picker.Dispose();
//				});
//			}
			
			Cleanup (picker);
		}

		public override void Canceled (UIImagePickerController picker)
		{
			this.tcs.TrySetCanceled();
			
			Cleanup (picker);
		}

		public void DisplayPopover (bool hideFirst = false)
		{
			if (Popover == null)
				return;

			float swidth = UIScreen.MainScreen.Bounds.Width;
			float sheight= UIScreen.MainScreen.Bounds.Height;

			float width = 400;
			float height = 300;

			if (this.orientation == null)
			{
				if (IsValidInterfaceOrientation (UIDevice.CurrentDevice.Orientation))
					this.orientation = UIDevice.CurrentDevice.Orientation;
				else
					this.orientation = GetDeviceOrientation (this.viewController.InterfaceOrientation);
			}

			float x, y;
			if (this.orientation == UIDeviceOrientation.LandscapeLeft || this.orientation == UIDeviceOrientation.LandscapeRight)
			{
				y = (swidth / 2) - (height / 2);
				x = (sheight / 2) - (width / 2);
			}
			else
			{
				x = (swidth / 2) - (width / 2);
				y = (sheight / 2) - (height / 2);
			}

			if (hideFirst && Popover.PopoverVisible)
				Popover.Dismiss (animated: false);

			Popover.PresentFromRect (new RectangleF (x, y, width, height), View, 0, animated: true);
		}

		private UIDeviceOrientation? orientation;
		private NSObject observer;
		private readonly UIViewController viewController;
		private readonly UIImagePickerControllerSourceType source;
		private readonly TaskCompletionSource<MediaFile> tcs = new TaskCompletionSource<MediaFile>();
		private readonly StoreCameraMediaOptions options;
		
		private void Cleanup (UIImagePickerController picker)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (observer);
			UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications();
			
			observer.Dispose();
			
			if (Popover != null)
			{
				Popover.Dismiss (animated: true);
				Popover.Dispose();
				Popover = null;
			}
			else
			{
				picker.DismissModalViewControllerAnimated (animated: true);
				picker.Dispose();
			}
		}

		private void DidRotate (NSNotification notice)
		{
			UIDevice device = (UIDevice)notice.Object;
			if (!IsValidInterfaceOrientation (device.Orientation) || Popover == null)
				return;
			if (this.orientation.HasValue && IsSameOrientationKind (this.orientation.Value, device.Orientation))
				return;

			if (UIDevice.CurrentDevice.CheckSystemVersion (6, 0))
			{
				if (!GetShouldRotate6 (device.Orientation))
					return;
			}
			else if (!GetShouldRotate (device.Orientation))
				return;

			UIDeviceOrientation? co = this.orientation;
			this.orientation = device.Orientation;

			if (co == null)
				return;

			DisplayPopover (hideFirst: true);
		}

		private bool GetShouldRotate (UIDeviceOrientation orientation)
		{
			UIInterfaceOrientation iorientation = UIInterfaceOrientation.Portrait;
			switch (orientation)
			{
				case UIDeviceOrientation.LandscapeLeft:
					iorientation = UIInterfaceOrientation.LandscapeLeft;
					break;
					
				case UIDeviceOrientation.LandscapeRight:
					iorientation = UIInterfaceOrientation.LandscapeRight;
					break;
					
				case UIDeviceOrientation.Portrait:
					iorientation = UIInterfaceOrientation.Portrait;
					break;
					
				case UIDeviceOrientation.PortraitUpsideDown:
					iorientation = UIInterfaceOrientation.PortraitUpsideDown;
					break;
					
				default: return false;
			}

			return this.viewController.ShouldAutorotateToInterfaceOrientation (iorientation);
		}

		private bool GetShouldRotate6 (UIDeviceOrientation orientation)
		{
			UIInterfaceOrientationMask mask = UIInterfaceOrientationMask.Portrait;
			switch (orientation)
			{
				case UIDeviceOrientation.LandscapeLeft:
					mask = UIInterfaceOrientationMask.LandscapeLeft;
					break;
					
				case UIDeviceOrientation.LandscapeRight:
					mask = UIInterfaceOrientationMask.LandscapeRight;
					break;
					
				case UIDeviceOrientation.Portrait:
					mask = UIInterfaceOrientationMask.Portrait;
					break;
					
				case UIDeviceOrientation.PortraitUpsideDown:
					mask = UIInterfaceOrientationMask.PortraitUpsideDown;
					break;
					
				default: return false; 
			}

			return this.viewController.GetSupportedInterfaceOrientations().HasFlag (mask);
		}

		private MediaFile GetPictureMediaFile (NSDictionary info, out Task<NSUrl> saveCompleted)
		{
			saveCompleted = null;

			var image = (UIImage)info[UIImagePickerController.EditedImage];
			if (image == null)
				image = (UIImage)info[UIImagePickerController.OriginalImage];

			string path = null;
			//if (this.options.Location == MediaFileStoreLocation.Local)
			//{
				path = GetOutputPath (MediaPicker.TypeImage, options.Directory ?? String.Empty, options.Name);

				using (FileStream fs = File.OpenWrite (path))
				using (Stream s = new NSDataStream (image.AsJPEG()))
				{
					byte[] buffer = new byte[20480];
					int len;
					while ((len = s.Read (buffer, 0, buffer.Length)) > 0)
						fs.Write (buffer, 0, len);

					fs.Flush();
				}
//			}
//			else if (this.options.Location == MediaFileStoreLocation.CameraRoll)
//			{
//				var saveTcs = new TaskCompletionSource<NSUrl>();
//				saveCompleted = saveTcs.Task;
//
//				ALAssetsLibrary library = new ALAssetsLibrary();
//				library.WriteImageToSavedPhotosAlbum (image.AsJPEG(), (NSDictionary)info[UIImagePickerController.MediaMetadata],
//					(u, e) =>
//					{
//						if (e != null)
//							saveTcs.SetException (new NSErrorException (e));
//						else
//							saveTcs.SetResult (u);
//
//						library.Dispose();
//					});
//			}

			Func<Stream> streamGetter = () =>
			{
				return File.OpenRead (path);
				//switch (this.options.Location)
				//{
				//    case MediaFileStoreLocation.CameraRoll:
				//        return new NSDataStream (image.AsJPEG());

				//    case MediaFileStoreLocation.Local:
				//        return File.OpenRead (path);
				//        break;

				//    default:
				//        throw new NotSupportedException();
				//}
			};

			return new MediaFile (path, streamGetter);
		}

		private MediaFile GetMovieMediaFile (NSDictionary info, out Task<NSUrl> saveCompleted)
		{
			saveCompleted = null;
			NSUrl url = (NSUrl)info[UIImagePickerController.MediaURL];
		
			string path = null;
			//if (this.options.Location == MediaFileStoreLocation.Local)
			//{
				path = GetOutputPath (MediaPicker.TypeMovie, options.Directory ?? String.Empty, this.options.Name);
				File.Move (url.Path, path);
//			}
//			else if (this.options.Location == MediaFileStoreLocation.CameraRoll)
//			{
//				var saveTcs = new TaskCompletionSource<NSUrl>();
//				saveCompleted = saveTcs.Task;
//
//				ALAssetsLibrary library = new ALAssetsLibrary();
//				library.WriteVideoToSavedPhotosAlbum (url, (u, e) =>
//				{
//					url.Dispose();
//					url = u;
//					
//					if (e == null)
//					{
//						library.AssetForUrl (url,
//							a =>
//							{
//								path = a.RepresentationForUti ("public.movie").Filename;
//								saveTcs.SetResult (u);
//							
//								library.Dispose();
//							},
//							ae =>
//							{
//								saveTcs.SetException (new NSErrorException (ae));
//								library.Dispose();
//							});
//					}
//					else
//						saveTcs.SetException (new NSErrorException (e));					
//				});
//			}

			Func<Stream> streamGetter = () =>
			{
				return File.OpenRead (path);
				//switch (this.options.Location)
				//{
				//    case MediaFileStoreLocation.CameraRoll:
				//    case MediaFileStoreLocation.Local:
				//        return File.OpenRead (path);

				//    default:
				//        throw new NotSupportedException();
				//}
			};
			
			return new MediaFile (path, streamGetter);
		}
		
		private static string GetUniquePath (string type, string path, string name)
		{
			bool isPhoto = (type == MediaPicker.TypeImage);
			string ext = Path.GetExtension (name);
			if (ext == String.Empty)
				ext = ((isPhoto) ? ".jpg" : ".mp4");

			name = Path.GetFileNameWithoutExtension (name);

			string nname = name + ext;
			int i = 1;
			while (File.Exists (Path.Combine (path, nname)))
				nname = name + "_" + (i++) + ext;

			return Path.Combine (path, nname);
		}

		private static string GetOutputPath (string type, string path, string name)
		{
			path = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), path);
			Directory.CreateDirectory (path);

			if (String.IsNullOrWhiteSpace (name))
			{
				string timestamp = DateTime.Now.ToString ("yyyMMdd_HHmmss");
				if (type == MediaPicker.TypeImage)
					name = "IMG_" + timestamp + ".jpg";
				else
					name = "VID_" + timestamp + ".mp4";
			}

			return Path.Combine (path, GetUniquePath (type, path, name));
		}
		
		private static bool IsValidInterfaceOrientation (UIDeviceOrientation self)
		{
			return (self != UIDeviceOrientation.FaceUp && self != UIDeviceOrientation.FaceDown && self != UIDeviceOrientation.Unknown);
		}
		
		private static bool IsSameOrientationKind (UIDeviceOrientation o1, UIDeviceOrientation o2)
		{
			if (o1 == UIDeviceOrientation.FaceDown || o1 == UIDeviceOrientation.FaceUp)
				return (o2 == UIDeviceOrientation.FaceDown || o2 == UIDeviceOrientation.FaceUp);
			if (o1 == UIDeviceOrientation.LandscapeLeft || o1 == UIDeviceOrientation.LandscapeRight)
				return (o2 == UIDeviceOrientation.LandscapeLeft || o2 == UIDeviceOrientation.LandscapeRight);
			if (o1 == UIDeviceOrientation.Portrait || o1 == UIDeviceOrientation.PortraitUpsideDown)
				return (o2 == UIDeviceOrientation.Portrait || o2 == UIDeviceOrientation.PortraitUpsideDown);
			
			return false;
		}
		
		private static UIDeviceOrientation GetDeviceOrientation (UIInterfaceOrientation self)
		{
			switch (self)
			{
				case UIInterfaceOrientation.LandscapeLeft:
					return UIDeviceOrientation.LandscapeLeft;
				case UIInterfaceOrientation.LandscapeRight:
					return UIDeviceOrientation.LandscapeRight;
				case UIInterfaceOrientation.Portrait:
					return UIDeviceOrientation.Portrait;
				case UIInterfaceOrientation.PortraitUpsideDown:
					return UIDeviceOrientation.PortraitUpsideDown;
				default:
					throw new InvalidOperationException();
			}
		}
	}
}