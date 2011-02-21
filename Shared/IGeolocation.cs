using System;

namespace MonoMobile.Extensions
{
    public interface IGeolocation
    {
        void GetCurrentPosition(Action<Position> success);
        void GetCurrentPosition(Action<Position> success, Action<PositionError> error);
        void GetCurrentPosition(Action<Position> success, Action<PositionError> error, GeolocationOptions options);

        string WatchPosition(Action<Position> success);
        string WatchPosition(Action<Position> success, Action<PositionError> error);
        string WatchPosition(Action<Position> success, Action<PositionError> error, GeolocationOptions options);

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
        public DateTimeOffset Timestamp { get; set; }

        public Position()
        {
            Coords = new Coordinates();
        }
    }

    public class Coordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double Accuracy { get; set; }
        public double AltitudeAccuracy { get; set; }
        public double Heading { get; set; }
        public double Speed { get; set; }
    }

    public class PositionError
    {
        public string Message { get; set; }
        public PositionErrorCode Code { get; set; }        

        public PositionError()
        {
            
        }

        public PositionError(PositionErrorCode code, string message)
        {
            Code = code;
            Message = message;
        }
    }

    public enum PositionErrorCode
    {
        PermissionDenied,
        PositionUnavailable,
        Timeout
    }
}