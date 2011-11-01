using System;
using MonoTouch.CoreLocation;
using System.Threading.Tasks;

namespace MonoMobile.Extensions
{
	internal class GeolocationSingleUpdateDelegate
		: CLLocationManagerDelegate
	{
		public GeolocationSingleUpdateDelegate (CLLocationManager manager)
		{
			this.locations = manager;
			this.tcs = new TaskCompletionSource<Position> (manager);
		}
		
		public Task<Position> Task
		{
			get { return this.tcs.Task; }
		}
		
		public override void AuthorizationChanged (CLLocationManager manager, CLAuthorizationStatus status)
		{
			// BUG: If user has services disables, but goes and reenables them fromt he prompt, this still cancels
			if (status == CLAuthorizationStatus.Denied || status == CLAuthorizationStatus.Restricted)
				this.tcs.TrySetCanceled();
		}
		
		public override void Failed (CLLocationManager manager, MonoTouch.Foundation.NSError error)
		{
			this.tcs.TrySetException (new GeolocationException (error.Domain + ": " + (CLError)error.Code));
		}
		
		public override void MonitoringFailed (CLLocationManager manager, CLRegion region, MonoTouch.Foundation.NSError error)
		{
			this.tcs.TrySetException (new GeolocationException (error.Domain + ": " + (CLError)error.Code));
		}
		
		public override void UpdatedLocation (CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation)
		{
			this.locations.StopUpdatingLocation();
			
			this.position.Coords.Altitude = this.locations.Location.Altitude;
			this.position.Coords.Latitude = this.locations.Location.Coordinate.Latitude;
			this.position.Coords.Longitude = this.locations.Location.Coordinate.Longitude;
			this.position.Coords.Speed = this.locations.Location.Speed;
			
			this.haveLocation = true;
			
			if (!CLLocationManager.HeadingAvailable || this.haveHeading)
				this.tcs.TrySetResult (this.position);
		}
		
		public override void UpdatedHeading (CLLocationManager manager, CLHeading newHeading)
		{
			this.locations.StopUpdatingHeading();
			
			this.position.Coords.Heading = newHeading.TrueHeading;
			
			this.haveHeading = true;
			
			if (this.haveLocation)
				this.tcs.TrySetResult (this.position);
		}
		
		private bool haveLocation;
		private bool haveHeading;
		
		private readonly Position position = new Position();
		private readonly TaskCompletionSource<Position> tcs;
		private readonly CLLocationManager locations;
	}
}

