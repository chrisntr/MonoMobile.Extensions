using System;
using System.Device.Location;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin.Geolocation
{
	public class Geolocator
	{
		public Geolocator()
		{
			this.isEnabled = GetEnabled();
		}

		public event EventHandler<PositionErrorEventArgs> PositionError;
		public event EventHandler<PositionEventArgs> PositionChanged;

		public bool IsGeolocationAvailable
		{
			get { return true; }
		}

		public bool IsGeolocationEnabled
		{
			get
			{
				if (this.watcher != null)
					this.isEnabled = (this.watcher.Permission == GeoPositionPermission.Granted && this.watcher.Status != GeoPositionStatus.Disabled);
				else
					this.isEnabled = GetEnabled();

				return this.isEnabled;
			}
		}

		public double DesiredAccuracy
		{
			get;
			set;
		}

		public bool SupportsHeading
		{
			get { return true; }
		}

		public bool IsListening
		{
			get { return (this.watcher != null); }
		}

		public Task<Position> GetPositionAsync (CancellationToken cancelToken)
		{
			return GetPositionAsync (Timeout.Infinite, cancelToken);
		}

		public Task<Position> GetPositionAsync (int timeout)
		{
			return GetPositionAsync (timeout, CancellationToken.None);
		}

		public Task<Position> GetPositionAsync (int timeout, CancellationToken cancelToken)
		{
			if (timeout <= 0 && timeout != Timeout.Infinite)
				throw new ArgumentOutOfRangeException ("timeout", "timeout must be greater than or equal to 0");

			return new SinglePositionListener (DesiredAccuracy, timeout, cancelToken).Task;
		}

		public void StartListening (int minTime, double minDistance)
		{
			if (minTime < 0)
				throw new ArgumentOutOfRangeException ("minTime");
			if (minDistance < 0)
				throw new ArgumentOutOfRangeException ("minDistance");
			if (IsListening)
				throw new InvalidOperationException ("This Geolocator is already listening");

			this.watcher = new GeoCoordinateWatcher (GetAccuracy (DesiredAccuracy));
			this.watcher.MovementThreshold = minDistance;
			this.watcher.PositionChanged += WatcherOnPositionChanged;
			this.watcher.StatusChanged += WatcherOnStatusChanged;
			this.watcher.Start();
		}

		public void StopListening()
		{
			if (this.watcher == null)
				return;

			this.watcher.PositionChanged -= WatcherOnPositionChanged;
			this.watcher.StatusChanged -= WatcherOnStatusChanged;
			this.watcher.Stop();
			this.watcher.Dispose();
			this.watcher = null;
		}

		private GeoCoordinateWatcher watcher;
		private bool isEnabled;

		private static bool GetEnabled()
		{
			GeoCoordinateWatcher w = new GeoCoordinateWatcher();
			try
			{
				w.Start (true);
				bool enabled = (w.Permission == GeoPositionPermission.Granted && w.Status != GeoPositionStatus.Disabled);
				w.Stop();

				return enabled;
			}
			catch (Exception)
			{
				return false;
			}
			finally
			{
				w.Dispose();
			}
		}

		private void WatcherOnStatusChanged (object sender, GeoPositionStatusChangedEventArgs e)
		{
			this.isEnabled = (this.watcher.Permission == GeoPositionPermission.Granted && this.watcher.Status != GeoPositionStatus.Disabled);

			GeolocationError error;
			switch (e.Status)
			{
				case GeoPositionStatus.Disabled:
					error = GeolocationError.Unauthorized;
					break;

				case GeoPositionStatus.NoData:
					error = GeolocationError.PositionUnavailable;
					break;

				default:
					return;
			}

			StopListening();

			var perror = PositionError;
			if (perror != null)
				perror (this, new PositionErrorEventArgs (error));
		}

		private void WatcherOnPositionChanged (object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
		{
			Position p = GetPosition (e.Position);
			if (p != null)
			{
				var pupdate = PositionChanged;
				if (pupdate != null)
					pupdate (this, new PositionEventArgs (p));
			}
		}

		internal static GeoPositionAccuracy GetAccuracy (double desiredAccuracy)
		{
			if (desiredAccuracy < 100)
				return GeoPositionAccuracy.High;

			return GeoPositionAccuracy.Default;
		}

		internal static Position GetPosition (GeoPosition<GeoCoordinate> position)
		{
			if (position.Location.IsUnknown)
				return null;

			var p = new Position();
			p.Accuracy = position.Location.HorizontalAccuracy;
			p.Longitude = position.Location.Longitude;
			p.Latitude = position.Location.Latitude;

			if (!Double.IsNaN (position.Location.VerticalAccuracy) && !Double.IsNaN (position.Location.Altitude))
			{
				p.AltitudeAccuracy = position.Location.VerticalAccuracy;
				p.Altitude = position.Location.Altitude;
			}

			if (!Double.IsNaN (position.Location.Course))
				p.Heading = position.Location.Course;

			if (!Double.IsNaN (position.Location.Speed))
				p.Speed = position.Location.Speed;

			p.Timestamp = position.Timestamp;
			
			return p;
		}
	}
}
