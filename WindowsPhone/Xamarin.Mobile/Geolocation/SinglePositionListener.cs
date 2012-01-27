using System;
using System.Device.Location;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin.Geolocation
{
	internal class SinglePositionListener
	{
		internal SinglePositionListener (double accuracy, int timeout, CancellationToken cancelToken)
		{
			cancelToken.Register (HandleTimeout, true);

			this.watcher = new GeoCoordinateWatcher (Geolocator.GetAccuracy (accuracy));
			this.watcher.PositionChanged += WatcherOnPositionChanged;
			this.watcher.StatusChanged += WatcherOnStatusChanged;

			this.watcher.Start();

			this.desiredAccuracy = accuracy;
			if (timeout != Timeout.Infinite)
				this.timeout = new Timer (HandleTimeout);

			Task.ContinueWith (Cleanup);
		}

		public Task<Position> Task
		{
			get { return this.tcs.Task; }
		}

		private GeoPosition<GeoCoordinate> bestPosition;
		private readonly GeoCoordinateWatcher watcher;
		private readonly double desiredAccuracy;
		private readonly Timer timeout;
		private readonly TaskCompletionSource<Position> tcs = new TaskCompletionSource<Position>();

		private void Cleanup (Task task)
		{
			if (this.timeout != null)
				this.timeout.Dispose();

			this.watcher.Stop();
			this.watcher.Dispose();
		}

		private void HandleTimeout (object state)
		{
			if (state != null && (bool)state)
				this.tcs.TrySetCanceled();

			if (this.bestPosition != null)
				this.tcs.TrySetResult (Geolocator.GetPosition (this.bestPosition));
			else
				this.tcs.TrySetCanceled();
		}

		private void WatcherOnStatusChanged (object sender, GeoPositionStatusChangedEventArgs e)
		{
			switch (e.Status)
			{
				case GeoPositionStatus.NoData:
					this.tcs.TrySetException (new GeolocationException (GeolocationError.PositionUnavailable));
					break;

				case GeoPositionStatus.Disabled:
					this.tcs.TrySetException (new GeolocationException (GeolocationError.Unauthorized));
					break;
			}
		}

		private void WatcherOnPositionChanged (object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
		{
			if (e.Position.Location.IsUnknown)
				return;

			if (e.Position.Location.HorizontalAccuracy > this.desiredAccuracy)
				this.tcs.TrySetResult (Geolocator.GetPosition (e.Position));

			if (this.bestPosition == null || e.Position.Location.HorizontalAccuracy > this.bestPosition.Location.HorizontalAccuracy)
				this.bestPosition = e.Position;
		}
	}
}