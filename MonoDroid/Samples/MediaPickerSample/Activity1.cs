using Android.App;
using Android.Widget;
using Android.OS;
using Xamarin.Media;

namespace MediaPickerSample
{
	[Activity(Label = "MediaPickerSample", MainLauncher = true, Icon = "@drawable/icon")]
	public class Activity1 : Activity
	{
		int count = 1;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.MyButton);

			button.Click += delegate
			{
				var picker = new MediaPicker (this);
				picker.PickPhotoAsync()
					.ContinueWith (t => RunOnUiThread (() =>
					{
						button.Text = t.Result.FilePath;
					}));
			};
		}
	}
}

