using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Locations;

namespace MonoMobile.Extensions
{
	public class Geolocation
		: IGeolocation
	{
		public Geolocation (LocationManager manager)
		{
			if (manager == null)
				throw new ArgumentNullException ("manager");

			this.manager = manager;
		}

		public event EventHandler<PositionEventArgs> PositionChanged;

		public bool IsListening
		{
			get { return this.listener != null; }
		}

		public double DesiredAccuracy
		{
			get;
			set;
		}

		public bool SupportsHeading
		{
			get
			{
				if (this.headingProvider == null || !this.manager.IsProviderEnabled (this.headingProvider))
				{
					Criteria c = new Criteria { BearingRequired = true };
					string providerName = this.manager.GetBestProvider (c, enabledOnly: true);

					LocationProvider provider = this.manager.GetProvider (providerName);

					if (provider.SupportsBearing())
					{
						this.headingProvider = providerName;
						return true;
					}
					else
					{
						this.headingProvider = null;
						return false;
					}
				}
				else
					return true;
			}
		}

		public bool IsGeolocationAvailable
		{
			get { return this.manager.GetProviders (enabledOnly: true).Count > 0; }
		}

		public Task<Position> GetCurrentPosition()
		{
			var tcs = new TaskCompletionSource<Position>();

			if (!IsListening)
			{
				string provider;
				if (!TryGetProvider (out provider))
					tcs.SetCanceled();
				else
				{
					var singleListener = new GeolocationSingleListener (this.manager);
					this.manager.RequestLocationUpdates (provider, 0, 0, singleListener);
					return singleListener.Task;
				}
			}
			else
			{
				lock (this.positionSync)
				{
					if (this.lastPosition == null)
					{
						EventHandler<PositionEventArgs> gotPosition = null;
						gotPosition = (s, e) =>
						{
							tcs.TrySetResult (e.Position);
							PositionChanged -= gotPosition;
						};

						PositionChanged += gotPosition;
					}
					else
						tcs.SetResult (this.lastPosition);
				}
			}

			return tcs.Task;
		}

		public void StartListening (int minTime, double minDistance)
		{
			if (minTime < 0)
				throw new ArgumentOutOfRangeException ("minTime");
			if (minDistance < 0)
				throw new ArgumentOutOfRangeException ("minDistance");
			if (IsListening)
				throw new InvalidOperationException ("This geolocation is already listening");

			this.listener = new GeolocationContinuousListener();
			this.listener.PositionChanged += OnListenerPositionChanged;

			string provider;
			if (!TryGetProvider (out provider))
				return;

			this.manager.RequestLocationUpdates (provider, minTime, (float) minDistance, listener);
		}

		public void StopListening()
		{
			if (this.listener == null)
				return;

			this.listener.PositionChanged -= OnListenerPositionChanged;
			this.listener = null;
		}

		private readonly LocationManager manager;
		private string headingProvider;
		private GeolocationContinuousListener listener;
		private readonly object positionSync = new object();
		private Position lastPosition;

		private void OnListenerPositionChanged (object sender, PositionEventArgs e)
		{
			if (!IsListening) // ignore anything that might come in afterwards
				return;

			lock (this.positionSync)
			{
				this.lastPosition = e.Position;

				var changed = PositionChanged;
				if (changed != null)
					changed (this, e);
			}
		}

		private bool TryGetProvider (out string provider)
		{
			provider = this.headingProvider;
			if (provider == null)
			{
			    provider = this.manager.GetBestProvider (new Criteria { BearingRequired = true }, enabledOnly: true);
				return (provider != null);
			}

			return true;
		}
	}
}