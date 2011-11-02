using System;
using MonoTouch.CoreLocation;

namespace MonoMobile.Extensions
{
	internal class GeolocationContinuousDelegate
		: CLLocationManagerDelegate
	{
		public GeolocationContinuousDelegate (CLLocationManager manager)
		{
			this.locations = manager;
			
			PositionListener = new PositionListener (pl =>
			{
				manager.StopUpdatingHeading();
				manager.StopUpdatingLocation();
			});
		}
		
		public PositionListener PositionListener
		{
			get;
			private set;
		}

		public override void UpdatedLocation (CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation)
		{
			this.position.Altitude = this.locations.Location.Altitude;
			this.position.Latitude = this.locations.Location.Coordinate.Latitude;
			this.position.Longitude = this.locations.Location.Coordinate.Longitude;
			this.position.Speed = this.locations.Location.Speed;
			this.position.Timestamp = new DateTimeOffset (this.locations.Location.Timestamp);
			
			PositionListener.OnNext (this.position);
		}

		public override void UpdatedHeading (CLLocationManager manager, CLHeading newHeading)
		{
			this.position.Heading = newHeading.TrueHeading;
			
			PositionListener.OnNext (this.position);
		}
		
		public override void Failed (CLLocationManager manager, MonoTouch.Foundation.NSError error)
		{
			PositionListener.OnCompleted();
		}
		
		public override void MonitoringFailed (CLLocationManager manager, CLRegion region, MonoTouch.Foundation.NSError error)
		{
			PositionListener.OnCompleted();
		}
		
		private readonly Position position = new Position();
		private readonly CLLocationManager locations;
	}
}

