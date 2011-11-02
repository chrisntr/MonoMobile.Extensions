using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Locations;

namespace MonoMobile.Extensions
{
	public class Geolocation
		: IGeolocation
	{
		private readonly LocationManager statusManager;
		private readonly Context context;
		private string headingProvider;

		public Geolocation (Context context)
		{
			if (context == null)
				throw new ArgumentNullException ("context");

			this.context = context;
			this.statusManager = (LocationManager)context.GetSystemService (Context.LocationService);
		}

		public bool SupportsHeading
		{
			get
			{
				if (this.headingProvider == null || !this.statusManager.IsProviderEnabled (this.headingProvider))
				{
					Criteria c = new Criteria { BearingRequired = true };
					string providerName = this.statusManager.GetBestProvider (c, enabledOnly: true);

					LocationProvider provider = this.statusManager.GetProvider (providerName);

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
			get { return this.statusManager.GetProviders (enabledOnly: true).Count > 0; }
		}

		public Task<Position> GetCurrentPosition()
		{
			return GetCurrentPosition (new GeolocationOptions());
		}

		public Task<Position> GetCurrentPosition (GeolocationOptions options)
		{
			var m = (LocationManager)this.context.GetSystemService (Context.LocationService);

			var listener = new GeolocationSingleListener (m);

			string provider = this.headingProvider;
			if (provider == null)
			{
			    provider = m.GetBestProvider (new Criteria { BearingRequired = true }, enabledOnly: true);
			    if (provider == null)
			    {
			        var tcs = new TaskCompletionSource<Position>();
			        tcs.SetCanceled();
			        return tcs.Task;
			    }
			}

			// TODO: Maybe the listener should be handed the options
			// and handle this itself.
			m.RequestLocationUpdates (provider, 250, (float)options.DistanceInterval, listener);

			return listener.Task;
		}

		public PositionListener GetPositionListener()
		{
			return GetPositionListener (new GeolocationOptions());
		}

		public PositionListener GetPositionListener (GeolocationOptions options)
		{
			var m = (LocationManager)this.context.GetSystemService (Context.LocationService);

			var listener = new GeolocationContinuousListener (m);

			string provider = this.headingProvider;
			if (provider == null)
				provider = m.GetBestProvider (new Criteria { BearingRequired = true }, enabledOnly: true);

			// TODO: Maybe the listener should be handed the options
			// and handle this itself.
			m.RequestLocationUpdates (provider, options.UpdateInterval, (float)options.DistanceInterval, listener);

			return listener.PositionListener;
		}
	}
}