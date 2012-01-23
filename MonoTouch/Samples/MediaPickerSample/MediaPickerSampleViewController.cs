using MonoTouch.UIKit;
using System.Drawing;
using System;
using MonoTouch.Foundation;
using Xamarin.Media;
using MonoTouch.MediaPlayer;

namespace MediaPickerSample
{
	public partial class MediaPickerSampleViewController : UIViewController
	{
		public MediaPickerSampleViewController () : base ("MediaPickerSampleViewController", null)
		{
		}
		
		MediaPicker picker;
		MPMoviePlayerViewController moviePlayer;
		
		partial void takePhotoBtnClicked (MonoTouch.Foundation.NSObject sender)
		{
			Console.WriteLine("takePhotoBtnClicked");
			
			picker = new MediaPicker ();
			picker.TakePhotoAsync (new StoreCameraMediaOptions
			{
				Name = "test.jpg",
				Directory = "MediaPickerSample"
			})
			.ContinueWith (t =>
			{
				InvokeOnMainThread( delegate {
					UIImage image = UIImage.FromFile(t.Result.Path);
					this.imageView.Image = image;	
				});
			});
		}

		partial void pickPhotoBtnClicked (MonoTouch.Foundation.NSObject sender)
		{
			Console.WriteLine("pickPhotoBtnClicked");
			picker = new MediaPicker ();
			picker.PickPhotoAsync ()
			.ContinueWith (t =>
			{
				InvokeOnMainThread( delegate {
					UIImage image = UIImage.FromFile(t.Result.Path);
					this.imageView.Image = image;	
				});
			});
		}

		partial void takeVideoBtnClicked (MonoTouch.Foundation.NSObject sender)
		{
			Console.WriteLine("takeVideoBtnClicked");
			picker = new MediaPicker ();
			picker.TakeVideoAsync (new StoreVideoOptions
			{
				Quality = VideoQuality.Medium,
				DesiredLength = new TimeSpan(0, 0, 30)
			})
			.ContinueWith (t =>
			{
				Console.WriteLine("File Path: {0}", t.Result.Path);
					
				InvokeOnMainThread( delegate {
					moviePlayer = new MPMoviePlayerViewController (NSUrl.FromFilename(t.Result.Path));
					moviePlayer.MoviePlayer.UseApplicationAudioSession = true;
		    		this.PresentMoviePlayerViewController(moviePlayer);
				});
			});
		}

		partial void pickVideoBtnClicked (MonoTouch.Foundation.NSObject sender)
		{
			Console.WriteLine("pickVideoBtnClicked");
			picker = new MediaPicker ();
			picker.PickVideoAsync ()
			.ContinueWith (t =>
			{
				Console.WriteLine("File Path: {0}", t.Result.Path);
					
				InvokeOnMainThread( delegate {
					moviePlayer = new MPMoviePlayerViewController (NSUrl.FromFilename(t.Result.Path));
					moviePlayer.MoviePlayer.UseApplicationAudioSession = true;
		    		this.PresentMoviePlayerViewController(moviePlayer);
				});
			});
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			//any additional setup after loading the view, typically from a nib.
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Release any retained subviews of the main view.
			// e.g. myOutlet = null;
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}
