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

			location.StartUpdatingLocation();
			if (CLLocationManager.HeadingAvailable)
				location.StartUpdatingHeading();
			
			return cldelegate.Task;
		}		

		public string WatchPosition (Action<Position> success)
		{
			throw new NotImplementedException ();
		}

		public string WatchPosition (Action<Position> success, Action<PositionError> error)
		{
			throw new NotImplementedException ();
		}

		public string WatchPosition (Action<Position> success, Action<PositionError> error, GeolocationOptions options)
		{
			throw new NotImplementedException ();
		}

		public void ClearWatch (string watchID)
		{
			throw new NotImplementedException ();
		}
	}
}