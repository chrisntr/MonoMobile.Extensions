using System;
using MonoTouch.CoreLocation;
using System.Threading.Tasks;
using System.Threading;

namespace MonoMobile.Extensions
{
	internal class GeolocationSingleUpdateDelegate
		: CLLocationManagerDelegate
	{
		public GeolocationSingleUpdateDelegate (CLLocationManager manager, double desiredAccuracy, int timeout, CancellationToken cancelToken)
		{
			this.manager = manager;
			this.tcs = new TaskCompletionSource<Position> (manager);
			this.desiredAccuracy = desiredAccuracy;
			
			if (timeout != Timeout.Infinite)
			{
				Timer t = null;
				t = new Timer (s =>
				{
					if (this.haveLocation)
						this.tcs.TrySetResult (new Position (this.position));
					else
						this.tcs.TrySetCanceled();

					t.Dispose();
				}, null, timeout, 0);
			}
			
			cancelToken.Register (() =>
			{
				this.tcs.TrySetCanceled();
				StopListening();
			});
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
			this.tcs.TrySetCanceled();
			StopListening();
		}
		
		public override void MonitoringFailed (CLLocationManager manager, CLRegion region, MonoTouch.Foundation.NSError error)
		{
			this.tcs.TrySetCanceled();
			StopListening();
		}
		
		public override bool ShouldDisplayHeadingCalibration (CLLocationManager manager)
		{
			return true;
		}
		
		public override void UpdatedLocation (CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation)
		{
			if (newLocation.HorizontalAccuracy < 0)
				return;
			
			if (this.haveLocation && newLocation.HorizontalAccuracy > this.position.Accuracy)
				return;
			
			this.position.Accuracy = newLocation.HorizontalAccuracy;
			this.position.Altitude = newLocation.Altitude;
			this.position.Latitude = newLocation.Coordinate.Latitude;
			this.position.Longitude = newLocation.Coordinate.Longitude;
			this.position.Speed = newLocation.Speed;
			this.position.Timestamp = new DateTimeOffset (newLocation.Timestamp);
					
			this.haveLocation = true;
			
			if (this.haveHeading && this.position.Accuracy <= this.desiredAccuracy)
			{
				this.tcs.TrySetResult (new Position (this.position));
				StopListening();
			}
		}
		
		public override void UpdatedHeading (CLLocationManager manager, CLHeading newHeading)
		{
			if (newHeading.HeadingAccuracy < 0)
				return;
			if (this.bestHeading != null && newHeading.HeadingAccuracy >= this.bestHeading.HeadingAccuracy)
				return;
			
			this.bestHeading = newHeading;
			this.position.Heading = newHeading.TrueHeading;
			this.haveHeading = true;
			
			if (this.haveLocation && this.position.Accuracy <= this.desiredAccuracy)
			{
				this.tcs.TrySetResult (new Position (this.position));
				StopListening();
			}
		}
		
		private bool haveHeading;
		private bool haveLocation;
		private readonly Position position = new Position();
		private CLHeading bestHeading;

		private readonly double desiredAccuracy;
		private readonly TaskCompletionSource<Position> tcs;
		private readonly CLLocationManager manager;
		
		private void StopListening()
		{
			if (CLLocationManager.HeadingAvailable)
				this.manager.StopUpdatingHeading();
			
			this.manager.StopUpdatingLocation();
		}
	}
}