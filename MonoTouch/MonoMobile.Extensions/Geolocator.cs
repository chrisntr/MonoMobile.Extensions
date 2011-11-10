using System;
using MonoTouch.CoreLocation;
using System.Threading.Tasks;
using System.Threading;
using MonoTouch.Foundation;

namespace Xamarin.Geolocation
{
	public class Geolocator
	{
		public Geolocator()
		{
			this.manager = GetManager();
			this.manager.AuthorizationChanged += OnAuthorizationChanged;
			this.manager.Failed += OnFailed;
			this.manager.UpdatedLocation += OnUpdatedLocation;
			this.manager.UpdatedHeading += OnUpdatedHeading;
		}
		
		/// <summary>
		/// Raised when there is an error retrieving position information.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Temporary errors will not be reported.
		/// </para>
		/// <para>
		/// If an error occurs, listening will be stopped automatically.
		/// </para>
		/// </remarks>
		/// <seealso cref="PositionErrorCode"/>
		public event EventHandler<PositionErrorEventArgs> PositionError;

		/// <summary>
		/// Raised when position information is updated.
		/// </summary>
		/// <seealso cref="Position"/>
		public event EventHandler<PositionEventArgs> PositionChanged;
		
		/// <summary>
		/// Gets or sets the desired accuracy in meters.
		/// </summary>
		public double DesiredAccuracy
		{
			get;
			set;
		}

		/// <summary>
		/// Gets whether the position is currently being listened to.
		/// </summary>
		public bool IsListening
		{
			get { return this.isListening; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="MonoMobile.Extensions.IGeolocation"/> supports heading.
		/// </summary>
		/// <value>
		/// <c>true</c> if supports heading; otherwise, <c>false</c>.
		/// </value>
		public bool SupportsHeading
		{
			get { return CLLocationManager.HeadingAvailable; }
		}

		/// <summary>
		/// Gets a value indicating whether geolocation services are available on the device.
		/// </summary>
		/// <value>
		/// <c>true</c> if geolocation services are available; otherwise, <c>false</c>.
		/// </value>
		public bool IsGeolocationAvailable
		{
			get { return true; } // all iOS devices support at least wifi geolocation
		}

		/// <summary>
		/// Gets a value indicating whether geolocation is available and enabled.
		/// </summary>
		/// <value>
		/// <c>true</c> if geolocation is available and enabled; otherwise, <c>false</c>.
		/// </value>
		public bool IsGeolocationEnabled
		{
			get { return CLLocationManager.LocationServicesEnabled; }
		}

		/// <summary>
		/// Gets a future for the current position.
		/// </summary>
		/// <param name="timeout">
		/// The time before the request should be automatically cancelled in milliseconds. <see cref="Timeout.Infinite"/> for no timeout.
		/// </param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> for the current position.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is &lt= 0 and not <see cref="Timeout.Infinite"/>.</exception>
		/// <remarks>
		/// <para>
		/// If the underlying OS needs to request location access permissions, it will occur automatically.
		/// If the user has disallowed location access (now or previously), the future will be canceled.
		/// </para>
		/// <para>
		/// The first position that is within the <see cref="DesiredAccuracy"/> will be returned. If no
		/// position matches that accuracy, once <paramref cref="timeout"/> is reached, the most accurate
		/// position will be returned. If the <paramref name="timeout"/> is reached with no position being
		/// acquired, the task will be canceled.
		/// </para>
		/// <para>
		/// If this <see cref="IGeolocation"/> currently <see cref="IsListening"/>, the future will be
		/// set immediately with the last retrieved position.
		/// </para>
		/// </remarks>
		public Task<Position> GetCurrentPosition (int timeout)
		{
			return GetCurrentPosition (timeout, CancellationToken.None);
		}

		/// <summary>
		/// Gets a future for the current position.
		/// </summary>
		/// <param name="cancelToken">A <see cref="CancellationToken"/> to cancel the position request.</param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> for the current position.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If the underlying OS needs to request location access permissions, it will occur automatically.
		/// If the user has disallowed location access (now or previously), the future will be canceled.
		/// </para>
		/// <para>
		/// The first position that is within the <see cref="DesiredAccuracy"/> will be returned.
		/// </para>
		/// <para>
		/// If this <see cref="IGeolocation"/> currently <see cref="IsListening"/>, the future will be
		/// set immediately with the last retrieved position.
		/// </para>
		/// </remarks>
		public Task<Position> GetCurrentPosition (CancellationToken cancelToken)
		{
			return GetCurrentPosition (Timeout.Infinite, cancelToken);
		}

		/// <summary>
		/// Gets a future for the current position.
		/// </summary>
		/// <param name="timeout">The time before the request should be automatically cancelled in milliseconds</param>
		/// <param name="cancelToken">A <see cref="CancellationToken"/> to cancel the position request.</param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> for the current position.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is &lt= 0 and not <see cref="Timeout.Infinite"/>.</exception>
		/// <remarks>
		/// <para>
		/// If the underlying OS needs to request location access permissions, it will occur automatically.
		/// If the user has disallowed location access (now or previously), the future will be canceled.
		/// </para>
		/// <para>
		/// The first position that is within the <see cref="DesiredAccuracy"/> will be returned. If no
		/// position matches that accuracy, once <paramref cref="timeout"/> is reached, the most accurate
		/// position will be returned. If the <paramref name="timeout"/> is reached with no position being acquired, the
		/// task will be canceled.
		/// </para>
		/// <para>
		/// If this <see cref="IGeolocation"/> currently <see cref="IsListening"/>, the future will be
		/// set immediately with the last retrieved position.
		/// </para>
		/// </remarks>
		public Task<Position> GetCurrentPosition (int timeout, CancellationToken cancelToken)
		{
			if (timeout <= 0 && timeout != Timeout.Infinite)
				throw new ArgumentOutOfRangeException ("timeout", "Timeout must be positive or Timeout.Infinite");

			TaskCompletionSource<Position> tcs;

			if (!IsListening)
			{
				var m = GetManager();

				tcs = new TaskCompletionSource<Position> (m);
				var singleListener = new GeolocationSingleUpdateDelegate (m, DesiredAccuracy, timeout, cancelToken);
				m.Delegate = singleListener;

				m.StartUpdatingLocation ();
				if (SupportsHeading)
					m.StartUpdatingHeading ();

				return singleListener.Task;
			}
			else
			{
				tcs = new TaskCompletionSource<Position>();
				if (this.position == null)
				{
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

		/// <summary>
		/// Starts listening to position changes with specified thresholds.
		/// </summary>
		/// <param name="minTime">A hint for the minimum time between position updates in milliseconds.</param>
		/// <param name="minDistance">A hint for the minimum distance between position updates in meters.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para>
		/// <paramref name="minTime"/> is &lt; 0.
		/// </para>
		/// <para>or</para>
		/// <para>
		/// <paramref name="minDistance"/> is &lt; 0.
		/// </para>
		/// </exception>
		/// <exception cref="InvalidOperationException">This geolocation already <see cref="IsListening"/>.</exception>
		/// <seealso cref="StopListening"/>
		/// <seealso cref="IsListening"/>
		/// <remarks>
		/// <para>
		/// If the underlying OS needs to request location access permissions, it will occur automatically.
		/// </para>
		/// <para>
		/// If a <see cref="PositionError"/> occurs, listening will be halted automatically.
		/// </para>
		/// </remarks>
		public void StartListening (int minTime, double minDistance)
		{
			if (minTime < 0)
				throw new ArgumentOutOfRangeException ("minTime");
			if (minDistance < 0)
				throw new ArgumentOutOfRangeException ("minDistance");
			if (this.isListening)
				throw new InvalidOperationException ("Already listening");
			
			this.isListening = true;
			manager.DesiredAccuracy = DesiredAccuracy;
			manager.DistanceFilter = minDistance;
			manager.StartUpdatingLocation ();
			
			if (CLLocationManager.HeadingAvailable)
				manager.StartUpdatingHeading ();
		}

		/// <summary>
		/// Stops listening to position changes.
		/// </summary>
		/// <seealso cref="StartListening"/>
		/// <remarks>
		/// If you stop listening before the first position has arrived, and have a pending call to
		/// <see cref="GetCurrentPosition"/>, it will be canceled.
		/// </remarks>
		public void StopListening ()
		{
			if (!this.isListening)
				return;

			this.isListening = false;
			if (CLLocationManager.HeadingAvailable)
				manager.StopUpdatingHeading ();
			
			manager.StopUpdatingLocation ();
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

		private void OnUpdatedLocation (object sender, CLLocationUpdatedEventArgs e)
		{
			Position p = (this.position == null) ? new Position () : new Position (this.position);
			
			if (e.NewLocation.HorizontalAccuracy > -1)
			{
				p.Accuracy = e.NewLocation.HorizontalAccuracy;
				p.Latitude = e.NewLocation.Coordinate.Latitude;
				p.Longitude = e.NewLocation.Coordinate.Longitude;
			}
			
			if (e.NewLocation.VerticalAccuracy > -1)
			{
				p.Altitude = e.NewLocation.Altitude;
				p.AltitudeAccuracy = e.NewLocation.VerticalAccuracy;
			}
			
			if (e.NewLocation.Speed > -1)
				p.Speed = e.NewLocation.Speed;
			
			p.Timestamp = new DateTimeOffset (e.NewLocation.Timestamp);

			this.position = p;

			OnPositionChanged (new PositionEventArgs (p));
		}
		
		private void OnFailed (object sender, MonoTouch.Foundation.NSErrorEventArgs e)
		{
			if ((CLError)e.Error.Code == CLError.Network)
				OnPositionError (new PositionErrorEventArgs (PositionErrorCode.PositionUnavailable));
		}

		private void OnAuthorizationChanged (object sender, CLAuthroziationChangedEventArgs e)
		{
			if (e.Status == CLAuthorizationStatus.Denied || e.Status == CLAuthorizationStatus.Restricted)
				OnPositionError (new PositionErrorEventArgs (PositionErrorCode.Unauthorized));
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