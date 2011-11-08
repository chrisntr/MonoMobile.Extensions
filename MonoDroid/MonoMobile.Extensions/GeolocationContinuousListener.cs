using System;
using Android.Locations;
using Android.OS;

namespace MonoMobile.Extensions
{
	internal class GeolocationContinuousListener
		: Java.Lang.Object, ILocationListener
	{
		public GeolocationContinuousListener (LocationManager manager, TimeSpan timePeriod)
		{
			this.manager = manager;
			this.timePeriod = timePeriod;
		}
		
		public event EventHandler<PositionEventArgs> PositionChanged;
		
		public void OnLocationChanged (Location location)
		{
			if (this.activeProvider != null && this.manager.IsProviderEnabled (this.activeProvider))
			{
				LocationProvider pr = this.manager.GetProvider (location.Provider);
				if (pr.Accuracy < this.manager.GetProvider (this.activeProvider).Accuracy
					|| (GetTimeSpan (location.Time) - GetTimeSpan (this.lastLocation.Time)) < timePeriod.Add (timePeriod))
				{
					return;
				}
			}

			this.activeProvider = location.Provider;
			this.lastLocation = location;

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

			var changed = PositionChanged;
			if (changed != null)
				changed (this, new PositionEventArgs (p));
		}

		public void OnProviderDisabled (string provider)
		{

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
					break;
			}
		}
		
		private LocationManager manager;
		
		private string activeProvider;
		private Location lastLocation;
		private TimeSpan timePeriod;
		
		private TimeSpan GetTimeSpan (long time)
		{
			return new TimeSpan (TimeSpan.TicksPerMillisecond * time);
		}
	}
}