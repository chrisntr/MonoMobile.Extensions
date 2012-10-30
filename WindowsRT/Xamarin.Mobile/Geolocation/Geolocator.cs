using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace Xamarin.Geolocation
{
    public class Geolocator
    {
		public Geolocator()
		{
			this.locator.StatusChanged += OnLocatorStatusChanged;
		}

		public event EventHandler<PositionEventArgs> PositionChanged;
		public event EventHandler<PositionErrorEventArgs> PositionError;

		public bool IsGeolocationAvailable
		{
			get
			{
				PositionStatus status = GetGeolocatorStatus();

				while (status == PositionStatus.Initializing)
				{
					Task.Delay (10).Wait();
					status = GetGeolocatorStatus();
				}

				return status != PositionStatus.NotAvailable;
			}
		}

		public bool IsGeolocationEnabled
		{
			get
			{
				PositionStatus status = GetGeolocatorStatus();

				while (status == PositionStatus.Initializing)
				{
					Task.Delay (10).Wait();
					status = GetGeolocatorStatus();
				}

				return status != PositionStatus.Disabled && status != PositionStatus.NotAvailable;
			}
		}

		public double DesiredAccuracy
		{
			get { return this.desiredAccuracy; }
			set
			{
				this.desiredAccuracy = value;
				GetGeolocator().DesiredAccuracy = (value < 100) ? PositionAccuracy.High : PositionAccuracy.Default;
			}
		}

		public bool IsListening
		{
			get { return this.isListening; }
		}

		public Task<Position> GetPositionAsync (int timeout)
		{
			return GetPositionAsync (timeout, includeHeading: false);
		}

		public Task<Position> GetPositionAsync (int timeout, bool includeHeading)
		{
			if (timeout < 0)
				throw new ArgumentOutOfRangeException ("timeout");

			// The built in timeout does not cancel, it throws an exception, so we'll setup our own.
			IAsyncOperation<Geoposition> pos = GetGeolocator().GetGeopositionAsync (TimeSpan.Zero, TimeSpan.FromDays (365));
			Timeout timer = new Timeout (timeout, pos.Cancel);

			var tcs = new TaskCompletionSource<Position>();

			pos.Completed = (op, s) =>
			{
				timer.Cancel();

				switch (s)
				{
					case AsyncStatus.Canceled:
						tcs.SetCanceled();
						break;
					case AsyncStatus.Completed:
						tcs.SetResult (GetPosition (op.GetResults()));
						break;
					case AsyncStatus.Error:
						Exception ex = op.ErrorCode;
						if (ex is UnauthorizedAccessException)
							ex = new GeolocationException (GeolocationError.Unauthorized, ex);
							
						tcs.SetException (ex);
						break;
				}
			};

			return tcs.Task;
		}

		public Task<Position> GetPositionAsync (CancellationToken token)
		{
			return GetPositionAsync (token, includeHeading: false);
		}

		public async Task<Position> GetPositionAsync (CancellationToken token, bool includeHeading)
		{
			try
			{
				IAsyncOperation<Geoposition> op = GetGeolocator().GetGeopositionAsync();
				token.Register (o => ((IAsyncOperation<Geoposition>)o).Cancel(), op);

				Geoposition pos = await op.AsTask (false);
				return GetPosition (pos);
			}
			catch (UnauthorizedAccessException)
			{
				throw new GeolocationException (GeolocationError.Unauthorized);
			}
		}

		public Task<Position> GetPositionAsync (int timeout, CancellationToken token)
		{
			return GetPositionAsync (timeout, token, includeHeading: false);
		}

		public Task<Position> GetPositionAsync (int timeout, CancellationToken token, bool includeHeading)
		{
			if (timeout < 0)
				throw new ArgumentOutOfRangeException ("timeout");

			IAsyncOperation<Geoposition> pos = GetGeolocator().GetGeopositionAsync (TimeSpan.FromTicks (0), TimeSpan.FromDays (365));
			token.Register (o => ((IAsyncOperation<Geoposition>)o).Cancel(), pos);

			Timeout timer = new Timeout (timeout, pos.Cancel);

			var tcs = new TaskCompletionSource<Position>();

			pos.Completed = (op, s) =>
			{
				timer.Cancel();

				switch (s)
				{
					case AsyncStatus.Canceled:
						tcs.SetCanceled();
						break;
					case AsyncStatus.Completed:
						tcs.SetResult (GetPosition (op.GetResults()));
						break;
					case AsyncStatus.Error:
						Exception ex = op.ErrorCode;
						if (ex is UnauthorizedAccessException)
							ex = new GeolocationException (GeolocationError.Unauthorized, ex);
							
						tcs.SetException (ex);
						break;
				}
			};

			return tcs.Task;
		}

		public void StartListening (int minTime, double minDistance)
		{
			StartListening (minTime, minDistance, includeHeading: false);
		}

		public void StartListening (int minTime, double minDistance, bool includeHeading)
		{
			if (minTime < 0)
				throw new ArgumentOutOfRangeException ("minTime");
			if (minTime < minDistance)
				throw new ArgumentOutOfRangeException ("minDistance");
			if (this.isListening)
				throw new InvalidOperationException();

			this.isListening = true;

			var loc = GetGeolocator();
			loc.PositionChanged += OnLocatorPositionChanged;
			loc.ReportInterval = (uint)minTime;
			loc.MovementThreshold = minDistance;
		}

		public void StopListening()
		{
			if (!this.isListening)
				return;

			this.locator.PositionChanged -= OnLocatorPositionChanged;
			this.isListening = false;
		}

	    private bool isListening;
		private double desiredAccuracy;
		private Windows.Devices.Geolocation.Geolocator locator = new Windows.Devices.Geolocation.Geolocator();

		private void OnLocatorStatusChanged (Windows.Devices.Geolocation.Geolocator sender, StatusChangedEventArgs e)
		{
			GeolocationError error;
			switch (e.Status)
			{
				case PositionStatus.Disabled:
					error = GeolocationError.Unauthorized;
					break;

				case PositionStatus.NoData:
					error = GeolocationError.PositionUnavailable;
					break;

				default:
					return;
			}

			if (this.isListening)
			{
				StopListening();
				OnPositionError (new PositionErrorEventArgs (error));
			}

			this.locator = null;
		}

		private void OnLocatorPositionChanged (Windows.Devices.Geolocation.Geolocator sender, PositionChangedEventArgs e)
		{
			OnPositionChanged (new PositionEventArgs (GetPosition (e.Position)));
		}

		private void OnPositionChanged (PositionEventArgs e)
		{
			var handler = this.PositionChanged;
			if (handler != null)
				handler (this, e);
		}

		private void OnPositionError (PositionErrorEventArgs e)
		{
			var handler = this.PositionError;
			if (handler != null)
				handler (this, e);
		}

		private Windows.Devices.Geolocation.Geolocator GetGeolocator()
		{
			var loc = this.locator;
			if (loc == null)
			{
				this.locator = new Windows.Devices.Geolocation.Geolocator();
				this.locator.StatusChanged += OnLocatorStatusChanged;
				loc = this.locator;
			}

			return loc;
		}

		private PositionStatus GetGeolocatorStatus()
		{
			var loc = GetGeolocator();
			return loc.LocationStatus;
		}

		private static Position GetPosition (Geoposition position)
		{
			var pos = new Position
			{
				Accuracy = position.Coordinate.Accuracy,
				Latitude = position.Coordinate.Latitude,
				Longitude = position.Coordinate.Longitude,
				Timestamp = position.Coordinate.Timestamp,
			};

			if (position.Coordinate.Heading != null)
				pos.Heading = position.Coordinate.Heading.Value;

			if (position.Coordinate.Speed != null)
				pos.Speed = position.Coordinate.Speed.Value;

			if (position.Coordinate.AltitudeAccuracy != null)
				pos.AltitudeAccuracy = position.Coordinate.AltitudeAccuracy.Value;

			if (position.Coordinate.Altitude != null)
				pos.Altitude = position.Coordinate.Altitude.Value;

			return pos;
		}
    }
}