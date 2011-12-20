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
	public class Activity1 : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			ImageView image = FindViewById<ImageView> (Resource.Id.image);

			Button videoButton = FindViewById<Button> (Resource.Id.videoButton);
			videoButton.Click += delegate
			{
				var picker = new MediaPicker (this);
				picker.TakeVideoAsync (new StoreVideoOptions
				{
					Name = "video",
					Description = "desc",
					Directory = "asdf",
					MaximumLength = TimeSpan.FromSeconds (10)
				})
				.ContinueWith (t =>
				{
					string l;
					using (Stream st = t.Result.GetStream())
						l = st.Length.ToString("N0");

					RunOnUiThread (() => videoButton.Text = l);
					if (t.Result.CanDelete)
						t.Result.Delete();
				});
			};

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