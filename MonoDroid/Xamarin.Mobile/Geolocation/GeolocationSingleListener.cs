using System;
using System.Threading.Tasks;
using Android.Locations;
using Android.OS;
using System.Threading;
using System.Collections.Generic;

namespace Xamarin.Geolocation
{
	internal class GeolocationSingleListener
		: Java.Lang.Object, ILocationListener
	{
		public GeolocationSingleListener (float desiredAccuracy, int timeout, IEnumerable<string> activeProviders, Action finishedCallback)
		{
			this.desiredAccuracy = desiredAccuracy;
			this.finishedCallback = finishedCallback;

			this.activeProviders = new HashSet<string> (activeProviders);

			if (timeout != Timeout.Infinite)
				this.timer = new Timer (TimesUp, null, timeout, 0);
		}

		public Task<Position> Task
		{
			get { return this.completionSource.Task; }
		}

		public void OnLocationChanged (Location location)
		{
			if (location.Accuracy <= this.desiredAccuracy)
			{
				Finish (location);
				return;
			}
			
			lock (this.locationSync)
			{
				if (this.bestLocation == null || location.Accuracy <= this.bestLocation.Accuracy)
					this.bestLocation = location;
			}
		}

		public void OnProviderDisabled (string provider)
		{
			lock (this.activeProviders)
			{
				if (this.activeProviders.Remove (provider) && this.activeProviders.Count == 0)
					this.completionSource.TrySetException (new GeolocationException (GeolocationError.PositionUnavailable));
			}
		}

		public void OnProviderEnabled (string provider)
		{
			lock (this.activeProviders)
				this.activeProviders.Add (provider);	
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
			switch (status)
			{
				case Availability.Available:
					OnProviderEnabled (provider);
					break;
				
				case Availability.OutOfService:
					OnProviderDisabled (provider);
					break;
			}
		}

		public void Cancel()
		{
			this.completionSource.TrySetCanceled();
		}
		
		private readonly object locationSync = new object();
		private Location bestLocation;
		
		private readonly Action finishedCallback;
		private readonly float desiredAccuracy;
		private readonly Timer timer;
		private readonly TaskCompletionSource<Position> completionSource = new TaskCompletionSource<Position>();
		private HashSet<string> activeProviders = new HashSet<string>();
		
		private void TimesUp (object state)
		{
			lock (this.locationSync)
			{
				if (this.bestLocation == null)
				{
					if (this.completionSource.TrySetCanceled() && this.finishedCallback != null)
						this.finishedCallback();
				}
				else
					Finish (this.bestLocation);
			}
		}
		
		private void Finish (Location location)
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
			
			if (this.finishedCallback != null)
				this.finishedCallback();

			this.completionSource.TrySetResult (p);
		}
	}
}