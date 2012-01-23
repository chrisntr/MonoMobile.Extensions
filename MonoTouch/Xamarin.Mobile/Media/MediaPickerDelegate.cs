using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MonoTouch.AssetsLibrary;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Xamarin.Media
{
	internal class MediaPickerDelegate
		: UIImagePickerControllerDelegate
	{
		internal MediaPickerDelegate (UIImagePickerControllerSourceType sourceType, StoreCameraMediaOptions options)
		{
			this.source = sourceType;
			this.options = options ?? new StoreCameraMediaOptions();
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

			picker.DismissModalViewControllerAnimated (animated: true);
			picker.Dispose();
		}

		public override void Canceled (UIImagePickerController picker)
		{
			this.tcs.TrySetCanceled();
			picker.DismissModalViewControllerAnimated (animated: true);
		}
		
		private readonly UIImagePickerControllerSourceType source;
		private readonly TaskCompletionSource<MediaFile> tcs = new TaskCompletionSource<MediaFile>();
		private readonly StoreCameraMediaOptions options;

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
//				switch (this.options.Location)
//				{
//					case MediaFileStoreLocation.CameraRoll:
//						return new NSDataStream (image.AsJPEG());
//
//					case MediaFileStoreLocation.Local:
//						return File.OpenRead (path);
//						break;
//
//					default:
//						throw new NotSupportedException();
//				}
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
//				switch (this.options.Location)
//				{
//					case MediaFileStoreLocation.CameraRoll:
//					case MediaFileStoreLocation.Local:
//						return File.OpenRead (path);
//
//					default:
//						throw new NotSupportedException();
//				}
			};
			
			return new MediaFile (path, streamGetter);
		}
		
		private static string GetUniquePath (string type, string path, string name)
		{
			bool isPhoto = (type == MediaPicker.TypeImage);
			string ext = Path.GetExtension (name);
			if (ext == String.Empty)
				ext = ((isPhoto) ? "jpg" : "mp4");

			name = Path.GetFileNameWithoutExtension (name);

			string nname = name + "." + ext;
			int i = 1;
			while (File.Exists (Path.Combine (path, nname)))
				nname = name + "_" + (i++) + "." + ext;

			return Path.Combine (path, nname);
		}

		private static string GetOutputPath (string type, string path, string name)
		{
			path = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), path);
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
	}
}