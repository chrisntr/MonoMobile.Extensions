using System;
using System.Linq;
using Android.Locations;
using Android.OS;
using Android.Util;
using Java.IO;

namespace MonoMobile.Extensions
{
   
    public class Geolocation : Java.Lang.Object, IGeolocation, ILocationListener
    {
        private readonly LocationManager _locationManager;

        private Action<Position> _success;
        private Action<PositionError> _error;
        private GeolocationOptions _options;
        private string _watchId;

        public Geolocation(LocationManager locationManager)
        {
            _locationManager = locationManager;
            _success = position => { };
            _error = error => { };
            _options= new GeolocationOptions();
            _watchId = Guid.NewGuid().ToString();
        }

        #region IGeolocation Members

        public void GetCurrentPosition(Action<Position> success)
        {
            GetCurrentPosition(success, (error)=> { });
        }

        public void GetCurrentPosition(Action<Position> success, Action<PositionError> error)
        {
            GetCurrentPosition(success, error, new GeolocationOptions());
        }

        public void GetCurrentPosition(Action<Position> success, Action<PositionError> error, GeolocationOptions options)
        {
            var criteria = new Criteria { Accuracy = options.EnableHighAccuracy ? Accuracy.Fine : Accuracy.Coarse};
            string bestProvider = _locationManager.GetBestProvider(criteria, true);
            Location lastKnownLocation=null;
            try
            {
                lastKnownLocation = _locationManager.GetLastKnownLocation(bestProvider);
            }
            catch (Exception exception)
            {
                error(new PositionError(PositionErrorCode.PositionUnavailable, exception.Message));
            }
            if (lastKnownLocation == null)
            {
                error(new PositionError(PositionErrorCode.PositionUnavailable,"No position aquired"));
            }

            SendLocation(lastKnownLocation, success);
            
        }

        public string WatchPosition(Action<Position> success)
        {
            return WatchPosition(success, error => { });
        }

        public string WatchPosition(Action<Position> success, Action<PositionError> error)
        {
            return WatchPosition(success,error,new GeolocationOptions());
        }

        public string WatchPosition(Action<Position> success, Action<PositionError> error, GeolocationOptions options)
        {
            _success = success;
            _error = error;
            _options = options;
            string provider = _options.EnableHighAccuracy ? LocationManager.GpsProvider : LocationManager.NetworkProvider;
            
 
            _locationManager.RequestLocationUpdates(provider,_options.MaximumAge, 50,this);

            return _watchId;
        }

        public void ClearWatch(string watchID)
        {
            if (watchID != _watchId)
                return;

            _locationManager.RemoveUpdates(this);
        }

        #endregion

        #region ILocationListener Members

        public void OnLocationChanged(Location location)
        {
            SendLocation(location,_success);
        }

        public void OnProviderDisabled(string provider)
        {
            //Dows this concern us ?
        }

        public void OnProviderEnabled(string provider)
        {
            //Dows this concern us ?
        }

        public void OnStatusChanged(string provider, int status, Bundle extras)
        {
            //Dows this concern us ?
        }


        #endregion

        private void SendLocation(Location lastKnownLocation, Action<Position> success)
        {
            Position pos=new Position();
            pos.Timestamp = DateTime.Now;
            if (lastKnownLocation != null)
            {
                pos.Coords = new Coordinates
                                 {
                                     Accuracy = lastKnownLocation.Accuracy,
                                     Heading = lastKnownLocation.Bearing,
                                     Altitude = lastKnownLocation.Altitude,
                                     AltitudeAccuracy = lastKnownLocation.Accuracy,
                                     Latitude = lastKnownLocation.Latitude,
                                     Longitude = lastKnownLocation.Longitude,
                                     Speed = lastKnownLocation.Speed
                                 };

            }
            success(pos);
        }
    }
}