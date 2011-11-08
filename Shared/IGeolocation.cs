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
		/// <param name="timeout">
		/// The time before the request should be automatically cancelled in milliseconds. <see cref="Timeout.Infinite"/> for no timeout.
		/// </param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> for the current position.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is &lt= 0 and not <see cref="Timeout.Infinite"/>.</exception>
		/// <remarks>
		/// <para>
		/// If the underlying OS needs to request location access permissions, it will occur automatically.
		/// If the user has disallowed location access (now or previously), the future will be canceled.
		/// </para>
		/// <para>
		/// The first position that is within the <see cref="DesiredAccuracy"/> will be returned. If no
		/// position matches that accuracy, once <paramref cref="timeout"/> is reached, the most accurate
		/// position will be returned. If the <paramref name="timeout"/> is reached with no position being
		/// acquired, the task will be canceled.
		/// </para>
		/// <para>
		/// If this <see cref="IGeolocation"/> currently <see cref="IsListening"/>, the future will be
		/// set immediately with the last retrieved position.
		/// </para>
		/// </remarks>
		Task<Position> GetCurrentPosition (int timeout);
		
		/// <summary>
		/// Gets a future for the current position.
		/// </summary>
		/// <param name="cancelToken">A <see cref="CancellationToken"/> to cancel the position request.</param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> for the current position.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If the underlying OS needs to request location access permissions, it will occur automatically.
		/// If the user has disallowed location access (now or previously), the future will be canceled.
		/// </para>
		/// <para>
		/// The first position that is within the <see cref="DesiredAccuracy"/> will be returned.
		/// </para>
		/// <para>
		/// If this <see cref="IGeolocation"/> currently <see cref="IsListening"/>, the future will be
		/// set immediately with the last retrieved position.
		/// </para>
		/// </remarks>
		Task<Position> GetCurrentPosition (CancellationToken cancelToken);
		
		/// <summary>
		/// Gets a future for the current position.
		/// </summary>
		/// <param name="timeout">The time before the request should be automatically cancelled in milliseconds</param>
		/// <param name="cancelToken">A <see cref="CancellationToken"/> to cancel the position request.</param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> for the current position.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is &lt= 0 and not <see cref="Timeout.Infinite"/>.</exception>
		/// <remarks>
		/// <para>
		/// If the underlying OS needs to request location access permissions, it will occur automatically.
		/// If the user has disallowed location access (now or previously), the future will be canceled.
		/// </para>
		/// <para>
		/// The first position that is within the <see cref="DesiredAccuracy"/> will be returned. If no
		/// position matches that accuracy, once <paramref cref="timeout"/> is reached, the most accurate
		/// position will be returned. If the <paramref name="timeout"/> is reached with no position being acquired, the
		/// task will be canceled.
		/// </para>
		/// <para>
		/// If this <see cref="IGeolocation"/> currently <see cref="IsListening"/>, the future will be
		/// set immediately with the last retrieved position.
		/// </para>
		/// </remarks>
		Task<Position> GetCurrentPosition (int timeout, CancellationToken cancelToken);

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
		/// <remarks>
		/// If you stop listening before the first position has arrived, and have a pending call to
		/// <see cref="GetCurrentPosition"/>, it will be canceled.
		/// </remarks>
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