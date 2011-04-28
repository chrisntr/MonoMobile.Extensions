using System;
using System.Linq;
using System.Timers;
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
        private Timer _timedWatch = new Timer();
        private bool _isTiming = false;

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
            try
            {
                if (options.EnableHighAccuracy && options.Timeout > 0)
                {
                    WatchPosition(success, error, options);
                }
                else
                {
                    Location lastKnownLocation=null;
                    if (options.EnableHighAccuracy)
                        lastKnownLocation = _locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
                    
                    if (lastKnownLocation==null)
                        lastKnownLocation = _locationManager.GetLastKnownLocation(LocationManager.NetworkProvider);
                    
                    SendLocation(lastKnownLocation, success);
                }
            }
            catch (Exception exception)
            {
                error(new PositionError(PositionErrorCode.PositionUnavailable, exception.Message));
            }
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
            TimeWatch();

            _locationManager.RequestLocationUpdates(provider,_options.MaximumAge, 50,this);

            return _watchId;
        }

        private void TimeWatch()
        {
            if (_options.Timeout > 0)
            {
                _timedWatch=new Timer(_options.Timeout);
                _timedWatch.Elapsed += (o, e) =>
                                           {
                                               if (_isTiming)
                                               {
                                                   _timedWatch.Stop();
                                                   ClearWatch(_watchId);
                                                   GetCurrentPosition(_success, _error,
                                                                      new GeolocationOptions()
                                                                          {
                                                                              EnableHighAccuracy = false,
                                                                              MaximumAge = _options.MaximumAge,
                                                                              Timeout = _options.Timeout
                                                                          });
                                                   _isTiming = false;
                                               }
                                           };
                _isTiming = true;
                _timedWatch.Start();
            }
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
            StopTiming();
        }

        private void StopTiming()
        {
            if (_isTiming)
            {
                _isTiming = false;
                _timedWatch.Stop();
                ClearWatch(_watchId);
            }
        }
    }
}