using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace MonoMobile.Extensions
{
	public class Camera
	{
		public class CameraOptions
		{
			public UIImagePickerControllerSourceType SourceType = UIImagePickerControllerSourceType.Camera;
			public bool AllowEdit = false;
			public int Quality = 100;
			public DestinationType destionation = DestinationType.FileUri;
		}
		
		public enum DestinationType
		{
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
				Console.WriteLine ("Source type {0} isn't available.", options.SourceType);
				return;
			}
			
			//if(pickerController == null)
				pickerController = new UIImagePickerController();
			
			pickerController.AllowsEditing = options.AllowEdit;
			pickerController.SourceType = options.SourceType;
			
			pickerController.FinishedPickingMedia += delegate(object sender, UIImagePickerMediaPickedEventArgs e) {
				
				pickerController.DismissModalViewControllerAnimated(true);
				
				float quality = options.Quality / 100.0f; 
				
				var mediaType = (NSString) e.Info[new NSString("UIImagePickerControllerMediaType")];
				if(mediaType == "public.image")
				{
				}
				
				// TODO: Finish implementation
			};
			pickerController.Canceled += delegate(object sender, EventArgs e) {
				
			};
			viewController.PresentModalViewController(pickerController, true);
			
		}
	}
}

