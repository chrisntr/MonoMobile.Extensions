using System;
using MonoTouch.CoreLocation;

namespace MonoMobile.Extensions
{
	internal class GeolocationContinuousDelegate
		: CLLocationManagerDelegate, IGeolocationListener
	{
		public GeolocationContinuousDelegate ()
		{
		}
		
		public override void UpdatedLocation (CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation)
		{
			
		}
		
		public override void UpdatedHeading (CLLocationManager manager, CLHeading newHeading)
		{
			
		}

		public IDisposable Subscribe (IObserver<Position> observer)
		{
			throw new NotImplementedException ();
		}
		
		public override void Dispose ()
		{
			throw new NotImplementedException ();
		}
		
		private readonly Position p = new Position();
	}
}

