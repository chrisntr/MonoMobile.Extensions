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
		/// <exception cref="ArgumentNullException"><paramref name="options"/> is <c>null</c>.</exception>
		/// <remarks>
		/// If the the underlying OS needs to request location access permissions, it will occur automatically.
		/// If the user has disallowed location access (now or previously), the future will be canceled.
		/// Any errors currently sets an exception on the future, but this needs to be reviewed as
		/// not all geolocation errors may be really be exceptional.
		/// </remarks>
		Task<Position> GetCurrentPosition (GeolocationOptions options);

		/// <summary>
		/// Gets a <see cref="PositionListener"/>.
		/// </summary>
		/// <returns>a <see cref="PositionListener"/>.</returns>
		PositionListener GetPositionListener();

		/// <summary>
		/// Gets a <see cref="PositionListener"/> for the given <paramref name="options"/>.
		/// </summary>
		/// <param name="options">Options for the listening to the current position.</param>
		/// <exception cref="ArgumentNullException"><paramref name="options"/> is <c>null</c>.</exception>
		/// <returns>a <see cref="PositionListener"/>.</returns>
		PositionListener GetPositionListener (GeolocationOptions options);
	}
    
    public class GeolocationOptions
    {
		/// <summary>
		/// Gets or sets the update internal in milliseconds.
		/// </summary>
		/// <remarks>
		/// The actual interval may be higher or lower than what's specified here, as
		/// it may only act as a hint to some implementations for power conservation.
		/// </remarks>
		public int UpdateInterval
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the distance interval in meters.
		/// </summary>
		public double DistanceInterval
		{
			get;
			set;
		}
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

    public enum PositionErrorCode
    {
        PermissionDenied,
        PositionUnavailable,
        Timeout
    }
}