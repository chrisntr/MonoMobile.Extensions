using System;
using System.IO;

namespace Xamarin.Media
{
	public class MediaFile
	{
		internal MediaFile (string path, Func<Stream> streamGetter)
		{
			if (streamGetter == null)
				throw new ArgumentNullException ("streamGetter");

			Path = path;
			this.streamGetter = streamGetter;
		}

		public string Path
		{
			get;
			private set;
		}

		public Stream GetStream()
		{
			return this.streamGetter();
		}

		private readonly Func<Stream> streamGetter;
	}
}