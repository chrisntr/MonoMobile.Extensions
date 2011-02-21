using System;

namespace MonoMobile.Extensions
{
    public interface IGeolocation
    {
        void GetCurrentPosition(Action<Position> geolocationSuccess);
        void GetCurrentPosition(Action<Position> geolocationSuccess, Action<PositionError> geolocationError);
        void GetCurrentPosition(Action<Position> geolocationSuccess, Action<PositionError> geolocationError, GeolocationOptions geolocationOptions);

        string WatchPosition(Action<Position> geolocationSuccess);
        string WatchPosition(Action<Position> geolocationSuccess, Action<PositionError> geolocationError);
        string WatchPosition(Action<Position> geolocationSuccess, Action<PositionError> geolocationError, GeolocationOptions geolocationOptions);

        void ClearWatch(string watchID);
    }
    
    public class GeolocationOptions
    {
        public int Timeout { get; set; }
        public int MaximumAge { get; set; }        
        public bool EnableHighAccuracy { get; set; }
    }

    public class Position
    {
        public Coordinates Coords { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class Coordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double Accuracy { get; set; }
        public double AltitudeAccuracy { get; set; }
        public double Heading { get; set; }
        public double Speet { get; set; }
    }

    public class PositionError
    {
        public PositionErrorCode Code { get; set; }
        public string Message { get; set; }
    }

    public enum PositionErrorCode
    {
        PermissionDenied,
        PositionUnavailable,
        Timeout
    }
}