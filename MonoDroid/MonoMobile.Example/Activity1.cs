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
			
			manager = (LocationManager)this.GetSystemService(LocationService);
			
			location = new Geolocator (manager);
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
				location.StartListening (5000, 1);
				
				watchButton.Text = GetString (Resource.String.watchStop);
							
//				Random r = new Random();
//				DateTime n = DateTime.Now;
//				Timer tt = null;
//				tt = new Timer (s =>
//				{
//					TimeSpan runtime = DateTime.Now - n;
//					if (runtime > TimeSpan.FromSeconds (20) && runtime < TimeSpan.FromSeconds (40))
//					{
//						this.manager.SetTestProviderLocation ("testGps", new Location ("testGps")
//						{
//							Latitude = r.Next(10, 100),
//							Longitude = r.Next (10, 100),
//							Accuracy = 10,
//							Time = (long)(DateTime.Now - new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds
//						});
//						
//						//tt.Dispose();
//					}
//					
//					this.manager.SetTestProviderLocation ("testWifi", new Location ("testWifi")
//					{
//						Latitude = r.Next (10, 100),
//						Longitude = r.Next (10, 100),
//						Accuracy = r.Next (60, 100),
//						Time = (long)(DateTime.Now - new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds
//					});
//				}, null, 5000, 5000);
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