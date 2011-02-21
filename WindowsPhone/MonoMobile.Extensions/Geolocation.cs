using System;
using System.Device.Location;

namespace MonoMobile.Extensions
{
    public class Geolocation : IGeolocation
    {
        private readonly string _watchId;
        private GeoCoordinateWatcher _geoWatcher;
        private Action<Position> _currentPositionCallback;
        private Action<Position> _watchPositionCallback;
        private Action<PositionError> _errorCallback;

        public Geolocation()
        {
            _watchId = Guid.NewGuid().ToString();
            _geoWatcher = new GeoCoordinateWatcher();
            _geoWatcher.StatusChanged += OnStatusChanged;
            _geoWatcher.PositionChanged += OnPositionChanged;
        }

        public void GetCurrentPosition(Action<Position> success, Action<PositionError> error, GeolocationOptions options)
        {
            _currentPositionCallback = success;
            _errorCallback = error;

            if(options.EnableHighAccuracy && _geoWatcher.DesiredAccuracy != GeoPositionAccuracy.High)
            {
                _geoWatcher.StatusChanged -= OnStatusChanged;
                _geoWatcher.Stop();

                _geoWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                _geoWatcher.StatusChanged += OnStatusChanged;
            }

            if(_geoWatcher.Status == GeoPositionStatus.Disabled || _geoWatcher.Status == GeoPositionStatus.NoData)
            {
                _geoWatcher.Start();
            }
        }

        private void OnStatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {            
            switch(e.Status)
            {
                case GeoPositionStatus.NoData:
                case GeoPositionStatus.Disabled:
                    var errorCode = _geoWatcher.Permission == GeoPositionPermission.Denied
                                        ? PositionErrorCode.PermissionDenied
                                        : PositionErrorCode.PositionUnavailable;
                    _errorCallback(new PositionError(errorCode, "TODO"));
                    _geoWatcher.Stop();
                    break;
                case GeoPositionStatus.Ready:                    
                    var position = CreatePosition(_geoWatcher.Position.Location);
                    position.Timestamp = _geoWatcher.Position.Timestamp;
                    _currentPositionCallback(position);
                    _geoWatcher.Stop();
                    break;
                default :
                    break;
            }
        }

        public string WatchPosition(Action<Position> success, Action<PositionError> error, GeolocationOptions options)
        {
            _watchPositionCallback = success;
            _errorCallback = error;
            if (options.EnableHighAccuracy && _geoWatcher.DesiredAccuracy != GeoPositionAccuracy.High)
            {
                _geoWatcher.PositionChanged -= OnPositionChanged;
                _geoWatcher.Stop();

                _geoWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                _geoWatcher.PositionChanged += OnPositionChanged;
            }

            _geoWatcher.Start();
            return _watchId;            
        }

        private void OnPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            var position = CreatePosition(e.Position.Location);
            position.Timestamp = e.Position.Timestamp;
            _watchPositionCallback(position);
        }

        public void ClearWatch(string watchID)
        {
            if (watchID == _watchId)
            {
                _geoWatcher.Stop();
            }            
        }

        private static Position CreatePosition(GeoCoordinate coordinate)
        {
            var position = new Position();
            var coords = position.Coords;

            coords.Altitude = coordinate.Altitude;
            coords.Latitude = coordinate.Latitude;
            coords.Longitude = coordinate.Longitude;
            coords.Speed = coordinate.Speed;
            coords.Accuracy = coordinate.HorizontalAccuracy;
            coords.AltitudeAccuracy = coordinate.VerticalAccuracy;
            return position;
        }

        public void GetCurrentPosition(Action<Position> success)
        {
            GetCurrentPosition(success, error => {}, new GeolocationOptions());
        }

        public void GetCurrentPosition(Action<Position> success, Action<PositionError> error)
        {
            GetCurrentPosition(success, error, new GeolocationOptions());
        }

        public string WatchPosition(Action<Position> success)
        {
            return WatchPosition(success, error => {}, new GeolocationOptions());
        }

        public string WatchPosition(Action<Position> success, Action<PositionError> error)
        {
            return WatchPosition(success, error, new GeolocationOptions());
        }              
    }
}
