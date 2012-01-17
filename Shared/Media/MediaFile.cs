using System;
using System.IO;

namespace Xamarin.Media
{
	public class MediaFile
	{
		internal MediaFile (string path, Func<Stream> streamGetter, Action delete)
		{
			if (path == null)
				throw new ArgumentNullException ("path");
			if (streamGetter == null)
				throw new ArgumentNullException ("streamGetter");

			Path = path;
			this.streamGetter = streamGetter;
			CanDelete = (delete != null);
			this.delete = delete;
		}

		public string Path
		{
			get;
			private set;
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