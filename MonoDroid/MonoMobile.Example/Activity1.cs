using System;
using Android.App;
using Android.Locations;
using Android.Widget;
using Android.OS;
using MonoMobile.Extensions;

namespace MonoMobile.Example
{
	[Activity(Label = "MonoMobile Android Example", MainLauncher = true)]
	public class Activity1 : Activity
	{
		IGeolocation location;
		bool watching = false;
		TextView locationTextView, currentLocationTextView;
		Button watchButton;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			location = new Geolocation ((LocationManager)this.GetSystemService(LocationService));
			//
			// Get our button from the layout resource,
			// and attach an event to it
			Button getLocationButton = FindViewById<Button>(Resource.Id.GetLocationButton);

			watchButton = FindViewById<Button>(Resource.Id.WatchButton);

			locationTextView = FindViewById<TextView>(Resource.Id.LocationTextView);
			currentLocationTextView = FindViewById<TextView>(Resource.Id.CurrentLocationTextView);

			getLocationButton.Click += delegate
								{
									LogDeviceInfo();
									GetCurrentPosition();
								};

			watchButton.Click += delegate { ToggleWatch(); };
		}

		private void GetCurrentPosition()
		{
			location.GetCurrentPosition().ContinueWith (t => RunOnUiThread (() =>
			{
				string message;
				if (t.IsCanceled)
					message = "Single: last canceled";
				else if (t.IsFaulted)
					message = "Single: last error: " + t.Exception;
				else
					message = "Single: last: " + GetText (t.Result);

				this.currentLocationTextView.Text = message;
			}));
		}

		private void ToggleWatch()
		{
			if (!watching)
			{
				location.PositionChanged += OnPostionChanged;
				location.StartListening (500, 1);
				
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

		private void LogDeviceInfo()
		{
			var device = new MonoMobile.Extensions.Device(this);
			Android.Util.Log.Info("MonoMobile.Extensions", "Device Name: {0}", device.Name);
			Android.Util.Log.Info("MonoMobile.Extensions", "Device Platform: {0}", device.Platform);
			Android.Util.Log.Info("MonoMobile.Extensions", "Device UUID: {0}", device.UUID);
			Android.Util.Log.Info("MonoMobile.Extensions", "Device Version: {0}", device.Version);
			Android.Util.Log.Info("MonoMobile.Extensions", "MonoMobile Version: {0}", device.MonoMobileVersion);
		}

		private string GetText (Position p)
		{
			return String.Format("{0} {1},{2} [{3}]",p.Timestamp, p.Latitude,
										   p.Longitude, p.Accuracy);
		}

		private void OnPostionChanged (object sender, PositionEventArgs e)
		{
			WatchSuccess (e.Position);
		}

		private void WatchSuccess(Position obj)
		{
			RunOnUiThread (() =>
			{
				string message = GetText (obj);
				Android.Util.Log.Info ("MonoMobile.Extension", message);
				locationTextView.Text = message;
			});
		}
	}
}