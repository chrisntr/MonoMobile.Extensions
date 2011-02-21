using System;

namespace MonoMobile.Extensions
{
    public class Geolocation : IGeolocation
    {
        public void GetCurrentPosition(Action<Position> geolocationSuccess)
        {
            throw new NotImplementedException();
        }

        public void GetCurrentPosition(Action<Position> geolocationSuccess, Action<PositionError> geolocationError)
        {
            throw new NotImplementedException();
        }

        public void GetCurrentPosition(Action<Position> geolocationSuccess, Action<PositionError> geolocationError, GeolocationOptions geolocationOptions)
        {
            throw new NotImplementedException();
        }

        public string WatchPosition(Action<Position> geolocationSuccess)
        {
            throw new NotImplementedException();
        }

        public string WatchPosition(Action<Position> geolocationSuccess, Action<PositionError> geolocationError)
        {
            throw new NotImplementedException();
        }

        public string WatchPosition(Action<Position> geolocationSuccess, Action<PositionError> geolocationError, GeolocationOptions geolocationOptions)
        {
            throw new NotImplementedException();
        }

        public void ClearWatch(string watchID)
        {
            throw new NotImplementedException();
        }
    }
}
