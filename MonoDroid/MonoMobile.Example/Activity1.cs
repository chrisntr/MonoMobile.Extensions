using System;
using Android.App;
using Android.Locations;
using Android.Widget;
using Android.OS;
using Xamarin.Geolocation;
using System.Threading;

namespace MonoMobile.Example
{
	[Activity(Label = "MonoMobile Android Example", MainLauncher = true)]
	public class Activity1 : Activity
	{
		bool watching = false;
		TextView locationTextView, currentLocationTextView;
		Button watchButton, cancelLocationButton, getLocationButton;

		private Geolocator location;
		private CancellationTokenSource cancelSource;
		private LocationManager manager;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			location = new Geolocator (this);
			location.DesiredAccuracy = 40;

			watchButton = FindViewById<Button>(Resource.Id.WatchButton);

			locationTextView = FindViewById<TextView>(Resource.Id.LocationTextView);
			currentLocationTextView = FindViewById<TextView>(Resource.Id.CurrentLocationTextView);
			
			cancelLocationButton = FindViewById<Button>(Resource.Id.CancelLocationButton);
			cancelLocationButton.Click += delegate { this.cancelSource.Cancel(); };

			getLocationButton = FindViewById<Button>(Resource.Id.GetLocationButton);
			getLocationButton.Click += delegate { GetCurrentPosition(); };

			watchButton.Click += delegate { ToggleWatch(); };
		}

		private void GetCurrentPosition()
		{
			this.cancelSource = new CancellationTokenSource();
			location.GetPositionAsync (30000, cancelSource.Token).ContinueWith (t =>
				RunOnUiThread (() =>
			{
				string message;
				if (t.IsCanceled)
					message = "Single: last canceled";
				else if (t.IsFaulted)
					message = "Single: last error: " + t.Exception;
				else
					message = "Single: last: " + GetText (t.Result);

				this.currentLocationTextView.Text = message;
				this.cancelSource = null;
			}));
		}

		private void ToggleWatch()
		{
			if (!watching)
			{
				location.PositionChanged += OnPostionChanged;
				location.PositionError += OnPositionError;
				location.StartListening (5000, 1);
				
				watchButton.Text = GetString (Resource.String.watchStop);
			}
			else
			{
				location.PositionChanged -= OnPostionChanged;
				watchButton.Text = GetString (Resource.String.watchStart);
				location.StopListening();
			}
			watching = !watching;
		}

		void OnPositionError (object sender, PositionErrorEventArgs e)
		{
			RunOnUiThread (() =>
			{
				Android.Util.Log.Error ("MonoMobile.Extension", e.Error.ToString());
				locationTextView.Text = "Error: " + e.Error;
			});
		}

		private string GetText (Position p)
		{
			return String.Format("{0} ({3}) {1},{2} {4}d",p.Timestamp, p.Latitude,
										   p.Longitude, p.Accuracy, p.Heading);
		}

		private void OnPostionChanged (object sender, PositionEventArgs e)
		{
			RunOnUiThread(() =>
			{
				string message = GetText (e.Position);
				Android.Util.Log.Info ("MonoMobile.Extension", message);
				locationTextView.Text = message;
			});
		}
	}
}