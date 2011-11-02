using System;
using MonoTouch.CoreLocation;
using System.Threading.Tasks;

namespace MonoMobile.Extensions
{
	public class Geolocation : IGeolocation
	{
		public bool SupportsHeading
		{
			get { return CLLocationManager.HeadingAvailable; }
		}

		public bool IsGeolocationAvailable
		{
			get { return CLLocationManager.LocationServicesEnabled; }
		}

		public Task<Position> GetCurrentPosition ()
		{
			return GetCurrentPosition (new GeolocationOptions());
		}

		public Task<Position> GetCurrentPosition (GeolocationOptions options)
		{
			// TODO: Timeout, prompt reason, should prompting be explicit?
			// Heading calibration prompt?

			CLLocationManager location = new CLLocationManager();

			var cldelegate = new GeolocationSingleUpdateDelegate (location);
			location.Delegate = cldelegate;
			
			location.DistanceFilter = options.DistanceInterval;
			location.StartUpdatingLocation();
			if (CLLocationManager.HeadingAvailable)
				location.StartUpdatingHeading();
			
			return cldelegate.Task;
		}
		
		public PositionListener GetPositionListener ()
		{
			return GetPositionListener (new GeolocationOptions());
		}

		public PositionListener GetPositionListener (GeolocationOptions options)
		{
			CLLocationManager location = new CLLocationManager();
			
			var cldelegate = new GeolocationContinuousDelegate (location);
			location.Delegate = cldelegate;
			
			location.DistanceFilter = options.DistanceInterval;
			
			location.StartUpdatingLocation();
			if (CLLocationManager.HeadingAvailable)
				location.StartUpdatingHeading();
			
			return cldelegate.PositionListener;
		}
	}
}