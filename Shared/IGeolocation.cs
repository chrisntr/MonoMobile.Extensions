using System;
using System.Threading.Tasks;

namespace MonoMobile.Extensions
{
	public interface IGeolocation
	{
		// TODO needs some kind of status change

		/// <summary>
		/// Raised when position information is updated.
		/// </summary>
		event EventHandler<PositionEventArgs> PositionChanged;

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
		/// Gets or sets the desired accuracy in meters.
		/// </summary>
		double DesiredAccuracy { get; set; }

		/// <summary>
		/// Gets whether the position is currently being listened to.
		/// </summary>
		bool IsListening { get; }

		/// <summary>
		/// Gets a future for the current position.
		/// </summary>
		/// <returns>
		/// A <see cref="Task{TResult}"/> for the current position.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If the underlying OS needs to request location access permissions, it will occur automatically.
		/// If the user has disallowed location access (now or previously), the future will be canceled.
		/// Any errors currently sets an exception on the future, but this needs to be reviewed as
		/// not all geolocation errors may be really be exceptional.
		/// </para>
		/// <para>
		/// If this <see cref="IGeolocation"/> currently <see cref="IsListening"/>, the future will be
		/// set immediately with the last retrieved position.
		/// </para>
		/// </remarks>
		Task<Position> GetCurrentPosition();

		/// <summary>
		/// Starts listening to position changes with specified thresholds.
		/// </summary>
		/// <param name="minTime">A hint for the minimum time between position updates in milliseconds.</param>
		/// <param name="minDistance">A hint for the minimum distance between position updates in meters.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para>
		/// <paramref name="minTime"/> is &lt; 0.
		/// </para>
		/// <para>or</para>
		/// <para>
		/// <paramref name="minDistance"/> is &lt; 0.
		/// </para>
		/// </exception>
		/// <exception cref="InvalidOperationException">This geolocation already <see cref="IsListening"/>.</exception>
		/// <seealso cref="StopListening"/>
		/// <seealso cref="IsListening"/>
		/// <remarks>
		/// If the underlying OS needs to request location access permissions, it will occur automatically.
		/// </remarks>
		void StartListening (int minTime, double minDistance);

		/// <summary>
		/// Stops listening to position changes.
		/// </summary>
		/// <seealso cref="StartListening"/>
		void StopListening();
	}

	public class PositionEventArgs
		: EventArgs
	{
		public PositionEventArgs (Position position)
		{
			if (position == null)
				throw new ArgumentNullException ("position");

			Position = position;
		}

		public Position Position
		{
			get;
			private set;
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