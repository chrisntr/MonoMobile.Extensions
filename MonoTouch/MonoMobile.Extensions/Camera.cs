using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MonoMobile.Extensions
{
	public class Camera
	{
		public class CameraOptions
		{
			// Set up Camera Option defaults
			public UIImagePickerControllerSourceType SourceType = UIImagePickerControllerSourceType.Camera;
			public bool AllowEdit = false;
			public int Quality = 100;
			public DestinationType Destination = DestinationType.FileUri;
		}
		
		public enum DestinationType
		{	
			// Might want to add a destination type of Object and return UIImage?
			FileUri,
			DataUri
		}

		public Camera (UIViewController currentViewController)
		{
			viewController = currentViewController;
		}
		
		UIViewController viewController;
		UIImagePickerController pickerController;
		
		public void GetPicture(CameraOptions options, Action<string> successCallback, Action errorCallback)
		{
			if(options == null)
				options = new CameraOptions();
		
			if(!UIImagePickerController.IsSourceTypeAvailable(options.SourceType))
			{
				Debug.WriteLine ("Source type {0} isn't available.", options.SourceType);
				return;
			}
			
			pickerController = new UIImagePickerController();
			
			pickerController.AllowsEditing = options.AllowEdit;
			pickerController.SourceType = options.SourceType;
			
			pickerController.FinishedPickingMedia += delegate(object sender, UIImagePickerMediaPickedEventArgs e) {
				
				pickerController.DismissModalViewControllerAnimated(true);
				
				float quality = options.Quality / 100.0f; 
				
				var mediaType = (NSString) e.Info[new NSString("UIImagePickerControllerMediaType")];
				if(mediaType == "public.image")
				{
					// Get the image
					UIImage image = null;
					if(pickerController.AllowsEditing && e.Info[new NSString("UIImagePickerControllerEditedImage")] != null)
						image = (UIImage) e.Info[new NSString("UIImagePickerControllerEditedImage")];
					else
						image = (UIImage) e.Info[new NSString("UIImagePickerControllerOriginalImage")];
						
					var data = image.AsJPEG(quality);
					
					if(options.Destination == DestinationType.FileUri)
					{
						var basedir = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), ".."); 
						var tmpDirectory = Path.Combine(basedir, "tmp");
						
						if (!Directory.Exists(tmpDirectory))
							Directory.CreateDirectory(tmpDirectory);
						
						string filePath;
						int i = 1;
						do {
							filePath = String.Format("{0}/photo_{1}.jpg", tmpDirectory, i++);
						}
						while (File.Exists(filePath));
						
						NSError error = null;
						if(!data.Save(filePath, false, out error))
						{
							Debug.WriteLine ("Error {0}", error.LocalizedDescription);
							errorCallback();	
						}
						else 
							successCallback(filePath);	
					}
					else
					{
						var bytearray = new Byte[data.Length];
						Marshal.Copy(data.Bytes, bytearray, 0, Convert.ToInt32(data.Length));
						successCallback(Convert.ToBase64String(bytearray));	
					}
				}
			};
			
			pickerController.Canceled += delegate(object sender, EventArgs e) {
				pickerController.DismissModalViewControllerAnimated(true);
				errorCallback();
			};
			
			viewController.PresentModalViewController(pickerController, true);
			
		}
	}
}

