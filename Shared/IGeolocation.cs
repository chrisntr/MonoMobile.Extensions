using System;
using System.Threading.Tasks;

namespace MonoMobile.Extensions
{
    public interface IGeolocation
    {
		/// <summary>
		/// Gets a value indicating whether this <see cref="MonoMobile.Extensions.IGeolocation"/> supports heading.
		/// </summary>
		/// <value>
		/// <c>true</c> if supports heading; otherwise, <c>false</c>.
		/// </value>
		bool SupportsHeading { get; }

		/// <summary>
		/// Gets a value indicating whether geolocation services are available.
		/// </summary>
		/// <value>
		/// <c>true</c> if geolocation services are available; otherwise, <c>false</c>.
		/// </value>
		bool IsGeolocationAvailable { get; }

		/// <summary>
		/// Gets a future for the current position.
		/// </summary>
		/// <returns>
		/// A <see cref="Task{TResult}"/> for the current position.
		/// </returns>
		/// <remarks>
		/// If the the underlying OS needs to request location access permissions, it will occur automatically.
		/// If the user has disallowed location access (now or previously), the future will be canceled.
		/// Any errors currently sets an exception on the future, but this needs to be reviewed as
		/// not all geolocation errors may be really be exceptional.
		/// </remarks>
		Task<Position> GetCurrentPosition();

		/// <summary>
		/// Gets a future for the current position.
		/// </summary>
		/// <param name="options">Options for getting the current position, currently ignored.</param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> for the current position.
		/// </returns>
		/// <remarks>
		/// If the the underlying OS needs to request location access permissions, it will occur automatically.
		/// If the user has disallowed location access (now or previously), the future will be canceled.
		/// Any errors currently sets an exception on the future, but this needs to be reviewed as
		/// not all geolocation errors may be really be exceptional.
		/// </remarks>
		Task<Position> GetCurrentPosition (GeolocationOptions options);

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
	
	public class GeolocationException
		: Exception
	{
		public GeolocationException()
			: base()
		{
		}
		
		public GeolocationException (string message)
			: base (message)
		{
		}
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