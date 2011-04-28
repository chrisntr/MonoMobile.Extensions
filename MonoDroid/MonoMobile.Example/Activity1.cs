using System;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.Runtime;
using Android.Views;
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
        string watchid = "";
        TextView locationTextView;
        Button watchButton;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

		    LocationManager locationManager=(LocationManager) GetSystemService(LocationService);
            
		    location = new Geolocation(locationManager);
		    //
			// Get our button from the layout resource,
			// and attach an event to it
            Button getLocationButton = FindViewById<Button>(Resource.Id.GetLocationButton);
            
		    watchButton = FindViewById<Button>(Resource.Id.WatchButton);

		    locationTextView = FindViewById<TextView>(Resource.Id.LocationTextView);

		    getLocationButton.Click += delegate
			                    {
			                        LogDeviceInfo();
			                        GetCurrentPosition();
			                    };

		    watchButton.Click += delegate { ToggleWatch(); };
		}

	    private void GetCurrentPosition()
	    {
	        location.GetCurrentPosition(
	            CurrentPositionSuccess, 
	            (error) => { },
	            new GeolocationOptions() {EnableHighAccuracy = true , Timeout = 1000}
	            );
	    }

	    
	    private void ToggleWatch()
	    {
	        if (!watching)
	        {   
	            watchid=location.WatchPosition(WatchSuccess);
                watchButton.Text = GetString(Resource.String.watchStop);
	        }
	        else
	        {
	            location.ClearWatch(watchid);
	            watchButton.Text = GetString(Resource.String.watchStart);
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

	    private void CurrentPositionSuccess(Position obj)
	    {
	        string message = string.Format("GetCurrentPosition location: {0} {1}-{2} [{3}]",obj.Timestamp, obj.Coords.Latitude,
	                                       obj.Coords.Longitude, obj.Coords.Accuracy);
            Android.Util.Log.Info("MonoMobile.Extensions",message);
	        locationTextView.Text = message;
	    }

	    private void WatchSuccess(Position obj)
	    {
            string message = string.Format("WatchPosition location: {0} {1}-{2} [{3}]",obj.Timestamp, obj.Coords.Latitude,
                                           obj.Coords.Longitude, obj.Coords.Accuracy);
            Android.Util.Log.Info("MonoMobile.Extension", message);
	        locationTextView.Text = message;
	    }
	}
}


