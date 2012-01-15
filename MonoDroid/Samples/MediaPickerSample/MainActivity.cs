using System;
using System.IO;
using Android.App;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using Xamarin.Media;
using Path = System.IO.Path;

namespace MediaPickerSample
{
	[Activity(Label = "MediaPickerSample", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
			ImageView image = FindViewById<ImageView> (Resource.Id.image);
			VideoView videoView  = FindViewById<VideoView>(Resource.Id.surfacevideoview);
			
			
			//
			// wire up the take a video button
			//
			Button videoButton = FindViewById<Button> (Resource.Id.videoButton);
			videoButton.Click += delegate
			{
				//
				// The MediaPicker is the class used to 
				// invoke the camera and gallery picker
				// for selecting and taking photos
				// and videos
				//
				var picker = new MediaPicker (this);
				
				//
				// TakeVideoAsync is an async API that takes a 
				// StoreVideoOptions object with various 
				// properies, such as the name and folder to
				// store the resulting video. You can
				// also limit the length of the video
				//
				picker.TakeVideoAsync (new StoreVideoOptions
				{
					Name = "MyVideo",
					Description = "This is a video from the Xamarin Mobile API Media Picker Sample",
					Directory = "MyVideos",
					DesiredLength = TimeSpan.FromSeconds (10)
				})
				.ContinueWith (t =>
				{
					//
					// Because TakeVideoAsync returns a Task
					// We can use ContinueWith to run more code
					// after the user finishes recording the video
					//
					   
					//
					// TODO requires filepath to video to be completely reliable
					//	
					// Load in the video file
					//videoView.SetVideoPath(videoFile);
        
					// Handle when the video finishes playing
        			//videoView.setOnCompletionListener(this);
        
					// Start playing the video
        			//videoView.Start();
				});
			};
			
			//
			// wire up the take a photo button
			//
			Button photoButton = FindViewById<Button> (Resource.Id.photoButton);
			photoButton.Click += delegate
			{
				var picker = new MediaPicker (this);
				picker.TakePhotoAsync (new StoreMediaOptions
				{
					Location = MediaFileStoreLocation.Local,
					Name = "test.jpg",
					Description = "Description!",
					Directory = "MediaPickerSample"
				})
				.ContinueWith (t =>
				{
					Bitmap b = BitmapFactory.DecodeStream (t.Result.GetStream());
					RunOnUiThread (() => image.SetImageBitmap (b));
				});
			};
		}
	}
}