using System;
using System.IO;

namespace Xamarin.Media
{
	public class MediaFile
	{
		internal MediaFile (Func<Stream> streamGetter, Action delete)
		{
			if (streamGetter == null)
				throw new ArgumentNullException ("streamGetter");

			this.streamGetter = streamGetter;
			CanDelete = (delete != null);
			this.delete = delete;
		}

		public bool CanDelete
		{
			get;
			private set;
		}

		public Stream GetStream()
		{
			return this.streamGetter();
		}

		public void Delete()
		{
			if (!CanDelete)
				throw new InvalidOperationException();

			this.delete();
			CanDelete = false;
		}

		private readonly Func<Stream> streamGetter;
		private readonly Action delete;
	}
}