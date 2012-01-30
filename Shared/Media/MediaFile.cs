using System;
using System.IO;

namespace Xamarin.Media
{
	public sealed class MediaFile
		: IDisposable
	{
		internal MediaFile (string path, Func<Stream> streamGetter, Action<bool> dispose = null)
		{
			this.dispose = dispose;
			this.streamGetter = streamGetter;
			this.path = path;
		}

		public string Path
		{
			get
			{
				if (this.isDisposed)
					throw new ObjectDisposedException (null);

				return this.path;
			}
		}

		public Stream GetStream()
		{
			if (this.isDisposed)
				throw new ObjectDisposedException (null);

			return this.streamGetter();
		}

		public void Dispose()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		private bool isDisposed;
		private readonly Action<bool> dispose;
		private readonly Func<Stream> streamGetter;
		private readonly string path;
		
		private void Dispose (bool disposing)
		{
			if (this.isDisposed)
				return;

			this.isDisposed = true;
			if (this.dispose != null)
				this.dispose (disposing);
		}

		~MediaFile()
		{
			Dispose (false);
		}
	}
}