using System;
using Android.Locations;
using Android.OS;
using System.Collections.Generic;

namespace MonoMobile.Extensions
{
	internal class GeolocationContinuousListener
		: Java.Lang.Object, ILocationListener
	{
		public GeolocationContinuousListener (LocationManager manager, TimeSpan timePeriod, IList<string> providers)
		{
			this.manager = manager;
			this.timePeriod = timePeriod;
			this.providers = providers;
			
			foreach (string p in providers)
			{
				if (manager.IsProviderEnabled (p))
					this.activeProviders.Add (p);
			}
		}
		
		public event EventHandler<PositionErrorEventArgs> PositionError;
		public event EventHandler<PositionEventArgs> PositionChanged;
		
		public void OnLocationChanged (Location location)
		{
			if (location.Provider != this.activeProvider)
			{
				if (this.activeProvider != null && this.manager.IsProviderEnabled (this.activeProvider))
				{
					LocationProvider pr = this.manager.GetProvider (location.Provider);
					TimeSpan lapsed = GetTimeSpan (location.Time) - GetTimeSpan (this.lastLocation.Time);
					
					if (pr.Accuracy > this.manager.GetProvider (this.activeProvider).Accuracy
						&& lapsed < timePeriod.Add (timePeriod))
					{
						return;
					}
				}
	
				this.activeProvider = location.Provider;
			}

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
			lock (this.activeProviders)
			{
				if (this.activeProviders.Remove (provider) && this.activeProviders.Count == 0)
					OnPositionError (new PositionErrorEventArgs (PositionErrorCode.PositionUnavailable));
			}
		}

		public void OnProviderEnabled (string provider)
		{
			lock (this.activeProviders)
				this.activeProviders.Add (provider);	
		}

		public void OnStatusChanged (string provider, int status, Bundle extras)
		{
			switch ((Availability)status)
			{
				case Availability.Available:
					OnProviderEnabled (provider);
					break;
				
				case Availability.OutOfService:
					OnProviderDisabled (provider);
					break;
			}
		}
		
		private LocationManager manager;
		private IList<string> providers;
		private HashSet<string> activeProviders = new HashSet<string>();
		
		private string activeProvider;
		private Location lastLocation;
		private TimeSpan timePeriod;
		
		private TimeSpan GetTimeSpan (long time)
		{
			return new TimeSpan (TimeSpan.TicksPerMillisecond * time);
		}
		
		private void OnPositionError (PositionErrorEventArgs e)
		{
			var error = PositionError;
			if (error != null)
				error (this, e);
		}
	}
}