using System;
using Android.App;
using Android.Locations;
using Android.Widget;
using Android.OS;
using MonoMobile.Extensions;
using System.Threading;

namespace MonoMobile.Example
{
	[Activity(Label = "MonoMobile Android Example", MainLauncher = true)]
	public class Activity1 : Activity
	{
		bool watching = false;
		TextView locationTextView, currentLocationTextView;
		Button watchButton, cancelLocationButton, getLocationButton;

		private IGeolocation location;
		private CancellationTokenSource cancelSource;
		private LocationManager manager;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			
			manager = (LocationManager)this.GetSystemService(LocationService);
			
			//this.manager.AddTestProvider ("testGps", false, true, false, false, false, false, false, 0, 10);
			//this.manager.SetTestProviderEnabled ("testGps", enabled: true);
			//this.manager.AddTestProvider ("testWifi", true, false, false, false, false, false, false, 0, 100);
			//this.manager.SetTestProviderEnabled ("testWifi", enabled: true);
			
			location = new Geolocation (manager);
			location.DesiredAccuracy = 40;
			
			watchButton = FindViewById<Button>(Resource.Id.WatchButton);

			locationTextView = FindViewById<TextView>(Resource.Id.LocationTextView);
			currentLocationTextView = FindViewById<TextView>(Resource.Id.CurrentLocationTextView);
			
			cancelLocationButton = FindViewById<Button>(Resource.Id.CancelLocationButton);
			cancelLocationButton.Click += delegate { this.cancelSource.Cancel(); };

			getLocationButton = FindViewById<Button>(Resource.Id.GetLocationButton);
			getLocationButton.Click += delegate
								{
									LogDeviceInfo();
									GetCurrentPosition();
								};

			watchButton.Click += delegate { ToggleWatch(); };
		}

		private void GetCurrentPosition()
		{
			
			this.cancelSource = new CancellationTokenSource();
			location.GetCurrentPosition (30000, cancelSource.Token).ContinueWith (t =>
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
			
//			DateTime n = DateTime.Now;
//			Timer tt = null;
//			tt = new Timer (s =>
//			{
//				if (DateTime.Now - n > TimeSpan.FromSeconds (20))
//				{
//					this.manager.SetTestProviderLocation ("testGps", new Location ("testGps")
//					{
//						Latitude = 50,
//						Longitude = 50,
//						Accuracy = 10
//					});
//					
//					tt.Dispose();
//				}
//				else
//				{
//					this.manager.SetTestProviderLocation ("testWifi", new Location ("testWifi")
//					{
//						Latitude = 70,
//						Longitude = 30,
//						Accuracy = 100
//					});
//				}
//			}, null, 5000, 5000);
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