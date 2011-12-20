using System.Collections.Generic;

namespace Xamarin.Media
{
	public class Photo
		: Media
	{
		internal Photo (IEnumerable<PhotoRepresentation> representations)
		{
			Representations = representations;
		}

		public IEnumerable<PhotoRepresentation> Representations
		{
			get;
			internal set;
		}
	}
}