using System;

namespace MonoMobile.Extensions
{
	public class Geolocation : IGeolocation
	{
		public void GetCurrentPosition (Action<Position> success)
		{
			throw new NotImplementedException ();
		}

		public void GetCurrentPosition (Action<Position> success, Action<PositionError> error)
		{
			throw new NotImplementedException ();
		}

		public void GetCurrentPosition (Action<Position> success, Action<PositionError> error, GeolocationOptions options)
		{
			throw new NotImplementedException ();
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