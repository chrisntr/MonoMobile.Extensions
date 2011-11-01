using System;
using System.Threading.Tasks;
using Android.Locations;
using Android.OS;

namespace MonoMobile.Extensions
{
	internal class GeolocationSingleListener
		: Java.Lang.Object, ILocationListener
	{
		public GeolocationSingleListener (LocationManager manager)
		{
			if (manager == null)
				throw new ArgumentNullException ("manager");

			this.manager = manager;
			this.manager.AddTestProvider ("test", true, true, false, false, false, false, true, 0, 10);
		}

		public Task<Position> Task
		{
			get { return this.completionSource.Task; }
		}

		public void OnLocationChanged (Location location)
		{
			StopListening();

			var p = new Position();
			if (location.HasAccuracy)
				p.Coords.Accuracy = location.Accuracy;
			if (location.HasAltitude)
				p.Coords.Altitude = location.Altitude;
			if (location.HasBearing)
				p.Coords.Heading = location.Bearing;
			if (location.HasSpeed)
				p.Coords.Speed = location.Speed;

			p.Coords.Longitude = location.Longitude;
			p.Coords.Latitude = location.Latitude;

			this.completionSource.TrySetResult (p);
		}

		public void OnProviderDisabled (string provider)
		{
			StopListening();
			this.completionSource.TrySetCanceled();
		}

		public void OnProviderEnabled (string provider)
		{
		}

		public void OnStatusChanged (string provider, int status, Bundle extras)
		{
			Availability av = (Availability) status;

			switch (av)
			{
				case Availability.OutOfService:
				case Availability.TemporarilyUnavailable:
					StopListening();
					this.completionSource.SetCanceled();
					break;
			}
		}

		private readonly LocationManager manager;
		private readonly TaskCompletionSource<Position> completionSource = new TaskCompletionSource<Position>();

		private void StopListening()
		{
			this.manager.RemoveUpdates (this);
		}
	}
}