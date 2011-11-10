using System;
using System.Threading.Tasks;
using Android.Locations;
using Android.OS;
using System.Threading;

namespace MonoMobile.Extensions
{
	internal class GeolocationSingleListener
		: Java.Lang.Object, ILocationListener
	{
		public GeolocationSingleListener (float desiredAccuracy, int timeout, Action callback)
		{
			this.desiredAccuracy = desiredAccuracy;
			this.callback = callback;
			
			if (timeout > 0)
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
					this.completionSource.TrySetCanceled();
					break;
			}
		}
		
		public void Cancel()
		{
			this.completionSource.TrySetCanceled();
		}
		
		private readonly object locationSync = new object();
		private Location bestLocation;
		
		private readonly Action callback;
		private readonly float desiredAccuracy;
		private readonly Timer timer;
		private readonly LocationManager manager;
		private readonly TaskCompletionSource<Position> completionSource = new TaskCompletionSource<Position>();
		
		private void TimesUp (object state)
		{
			lock (this.locationSync)
			{
				if (this.bestLocation == null)
				{
					this.completionSource.SetCanceled();
					if (this.callback == null)
						this.callback();
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
			
			if (this.callback != null)
				this.callback();

			this.completionSource.TrySetResult (p);
		}
	}
}