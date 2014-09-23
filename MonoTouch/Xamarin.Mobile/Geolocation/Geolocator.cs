//
//  Copyright 2011-2013, Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using System;
using System.Threading.Tasks;
using System.Threading;

#if __UNIFIED__
using CoreLocation;
using Foundation;
using UIKit;
#else
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

namespace Xamarin.Geolocation
{
	public class Geolocator
	{
		public Geolocator()
		{
			this.manager = GetManager();
			this.manager.AuthorizationChanged += OnAuthorizationChanged;
			this.manager.Failed += OnFailed;

			if (UIDevice.CurrentDevice.CheckSystemVersion (6, 0))
				this.manager.LocationsUpdated += OnLocationsUpdated;
			else
				this.manager.UpdatedLocation += OnUpdatedLocation;

			this.manager.UpdatedHeading += OnUpdatedHeading;

            RequestAuthorization ();
		}

        public void RequestAuthorization ()
        {
            var info = NSBundle.MainBundle.InfoDictionary;

            if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
                if (info.ContainsKey (new NSString ("NSLocationWhenInUseUsageDescription")))
                    this.manager.RequestWhenInUseAuthorization ();
                else if (info.ContainsKey (new NSString ("NSLocationAlwaysUsageDescription")))
                    this.manager.RequestAlwaysAuthorization ();
                else
                    throw new UnauthorizedAccessException ("On iOS 8.0 and higher you must set either NSLocationWhenInUseUsageDescription or NSLocationAlwaysUsageDescription in your Info.plist file to enable Authorization Requests for Location updates!");
            }
        }

		public event EventHandler<PositionErrorEventArgs> PositionError;

		public event EventHandler<PositionEventArgs> PositionChanged;

		public double DesiredAccuracy
		{
			get;
			set;
		}

		public bool IsListening
		{
			get { return this.isListening; }
		}

		public bool SupportsHeading
		{
			get { return CLLocationManager.HeadingAvailable; }
		}

		public bool IsGeolocationAvailable
		{
			get { return true; } // all iOS devices support at least wifi geolocation
		}

		public bool IsGeolocationEnabled
		{
			get { return CLLocationManager.Status == CLAuthorizationStatus.Authorized; }
		}

		public Task<Position> GetPositionAsync (int timeout)
		{
			return GetPositionAsync (timeout, CancellationToken.None, false);
		}

		public Task<Position> GetPositionAsync (int timeout, bool includeHeading)
		{
			return GetPositionAsync (timeout, CancellationToken.None, includeHeading);
		}

		public Task<Position> GetPositionAsync (CancellationToken cancelToken)
		{
			return GetPositionAsync (Timeout.Infinite, cancelToken, false);
		}

		public Task<Position> GetPositionAsync (CancellationToken cancelToken, bool includeHeading)
		{
			return GetPositionAsync (Timeout.Infinite, cancelToken, includeHeading);
		}

		public Task<Position> GetPositionAsync (int timeout, CancellationToken cancelToken)
		{
			return GetPositionAsync (timeout, cancelToken, false);
		}

		public Task<Position> GetPositionAsync (int timeout, CancellationToken cancelToken, bool includeHeading)
		{
			if (timeout <= 0 && timeout != Timeout.Infinite)
				throw new ArgumentOutOfRangeException ("timeout", "Timeout must be positive or Timeout.Infinite");

			TaskCompletionSource<Position> tcs;
			if (!IsListening)
			{
				var m = GetManager();

				tcs = new TaskCompletionSource<Position> (m);
				var singleListener = new GeolocationSingleUpdateDelegate (m, DesiredAccuracy, includeHeading, timeout, cancelToken);
				m.Delegate = singleListener;

				m.StartUpdatingLocation ();
				if (includeHeading && SupportsHeading)
					m.StartUpdatingHeading ();

				return singleListener.Task;
			}
			else
			{
				tcs = new TaskCompletionSource<Position>();
				if (this.position == null)
				{
					EventHandler<PositionErrorEventArgs> gotError = null;
					gotError = (s,e) =>
					{
						tcs.TrySetException (new GeolocationException (e.Error));
						PositionError -= gotError;
					};
					
					PositionError += gotError;
					
					EventHandler<PositionEventArgs> gotPosition = null;
					gotPosition = (s, e) =>
					{
						tcs.TrySetResult (e.Position);
						PositionChanged -= gotPosition;
					};

					PositionChanged += gotPosition;
				}
				else
					tcs.SetResult (this.position);
			}

			return tcs.Task;
		}

		public void StartListening (int minTime, double minDistance)
		{
			StartListening (minTime, minDistance, false);
		}

		public void StartListening (int minTime, double minDistance, bool includeHeading)
		{
			if (minTime < 0)
				throw new ArgumentOutOfRangeException ("minTime");
			if (minDistance < 0)
				throw new ArgumentOutOfRangeException ("minDistance");
			if (this.isListening)
				throw new InvalidOperationException ("Already listening");

			this.isListening = true;
			this.manager.DesiredAccuracy = DesiredAccuracy;
			this.manager.DistanceFilter = minDistance;
			this.manager.StartUpdatingLocation ();

			if (includeHeading && CLLocationManager.HeadingAvailable)
				this.manager.StartUpdatingHeading ();
		}

		public void StopListening ()
		{
			if (!this.isListening)
				return;

			this.isListening = false;
			if (CLLocationManager.HeadingAvailable)
				this.manager.StopUpdatingHeading ();

			this.manager.StopUpdatingLocation ();
			this.position = null;
		}

		private readonly CLLocationManager manager;
		private bool isListening;
		private Position position;

		private CLLocationManager GetManager()
		{
			CLLocationManager m = null;
			new NSObject().InvokeOnMainThread (() => m = new CLLocationManager());
			return m;
		}
		
		private void OnUpdatedHeading (object sender, CLHeadingUpdatedEventArgs e)
		{
			if (e.NewHeading.TrueHeading == -1)
				return;

			Position p = (this.position == null) ? new Position () : new Position (this.position);

			p.Heading = e.NewHeading.TrueHeading;

			this.position = p;
			
			OnPositionChanged (new PositionEventArgs (p));
		}

		private void OnLocationsUpdated (object sender, CLLocationsUpdatedEventArgs e)
		{
			foreach (CLLocation location in e.Locations)
				UpdatePosition (location);
		}

		private void OnUpdatedLocation (object sender, CLLocationUpdatedEventArgs e)
		{
			UpdatePosition (e.NewLocation);
		}

		private void UpdatePosition (CLLocation location)
		{
			Position p = (this.position == null) ? new Position () : new Position (this.position);
			
			if (location.HorizontalAccuracy > -1)
			{
				p.Accuracy = location.HorizontalAccuracy;
				p.Latitude = location.Coordinate.Latitude;
				p.Longitude = location.Coordinate.Longitude;
			}
			
			if (location.VerticalAccuracy > -1)
			{
				p.Altitude = location.Altitude;
				p.AltitudeAccuracy = location.VerticalAccuracy;
			}
			
			if (location.Speed > -1)
				p.Speed = location.Speed;
			
			p.Timestamp = new DateTimeOffset (location.Timestamp);
			
			this.position = p;
			
			OnPositionChanged (new PositionEventArgs (p));

			location.Dispose();
		}
		
		private void OnFailed (object sender, NSErrorEventArgs e)
		{
			if ((CLError)(int)e.Error.Code == CLError.Network)
				OnPositionError (new PositionErrorEventArgs (GeolocationError.PositionUnavailable));
		}

		private void OnAuthorizationChanged (object sender, CLAuthorizationChangedEventArgs e)
		{
			if (e.Status == CLAuthorizationStatus.Denied || e.Status == CLAuthorizationStatus.Restricted)
				OnPositionError (new PositionErrorEventArgs (GeolocationError.Unauthorized));
		}
		
		private void OnPositionChanged (PositionEventArgs e)
		{
			var changed = PositionChanged;
			if (changed != null)
				changed (this, e);
		}
		
		private void OnPositionError (PositionErrorEventArgs e)
		{
			StopListening();
			
			var error = PositionError;
			if (error != null)
				error (this, e);
		}
	}
}