using Android.App;
using Android.Widget;
using Android.OS;
using Xamarin.Media;

namespace MediaPickerSample
{
	[Activity(Label = "MediaPickerSample", MainLauncher = true, Icon = "@drawable/icon")]
	public class Activity1 : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			Button videoButton = FindViewById<Button> (Resource.Id.videoButton);
			videoButton.Click += delegate
			{
				var picker = new MediaPicker (this);
				picker.PickVideoAsync()
					.ContinueWith (t => RunOnUiThread (() =>
					{
						videoButton.Text = t.Result.FilePath;
					}));
			};

			Button photoButton = FindViewById<Button> (Resource.Id.photoButton);
			photoButton.Click += delegate
			{
				var picker = new MediaPicker (this);
				picker.PickPhotoAsync()
					.ContinueWith (t => RunOnUiThread (() =>
					{
						photoButton.Text = t.Result.FilePath;
					}));
			};
		}
	}
}

