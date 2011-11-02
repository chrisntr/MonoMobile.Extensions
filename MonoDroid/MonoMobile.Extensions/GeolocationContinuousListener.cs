using System;
using Android.Locations;
using Android.OS;

namespace MonoMobile.Extensions
{
	public class GeolocationContinuousListener
		: Java.Lang.Object, ILocationListener
	{
		public GeolocationContinuousListener (LocationManager locationManager)
		{
			PositionListener = new PositionListener (pl => locationManager.RemoveUpdates (this));
		}

		public PositionListener PositionListener
		{
			get;
			private set;
		}

		public void OnLocationChanged (Location location)
		{
			var p = new Position();
			if (location.HasAccuracy)
				p.Accuracy = location.Accuracy;
			if (location.HasAltitude)
				p.Altitude = location.Altitude;
			if (location.HasBearing)
				p.Heading = location.Bearing;
			if (location.HasSpeed)
				p.Speed = location.Speed;

			p.Longitude = location.Longitude;
			p.Latitude = location.Latitude;
			p.Timestamp = new DateTimeOffset (new DateTime (TimeSpan.TicksPerMillisecond * location.Time, DateTimeKind.Utc));

			PositionListener.OnNext (p);
		}

		public void OnProviderDisabled (string provider)
		{
			PositionListener.OnCompleted();
		}

		public void OnProviderEnabled (string provider)
		{
		}

		public void OnStatusChanged (string provider, int status, Bundle extras)
		{
			switch ((Availability)status)
			{
				case Availability.OutOfService:
				case Availability.TemporarilyUnavailable:
					PositionListener.OnCompleted();
					break;
			}
		}
	}
}