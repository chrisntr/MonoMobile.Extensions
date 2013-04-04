using System;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using File = System.IO.File;
using IOException = System.IO.IOException;

namespace Xamarin.Media
{
	public static class MediaFileExtensions
	{
		public static Task<MediaFile> GetMediaFileExtraAsync (this Intent self, Context context)
		{
			var uri = (Android.Net.Uri)self.GetParcelableExtra (MediaFile.ExtraName);
			bool isPhoto = self.GetBooleanExtra ("isPhoto", false);
			string action = self.GetStringExtra ("action");
			var path = (Android.Net.Uri)self.GetParcelableExtra ("path");

			return MediaPickerActivity.GetMediaFileAsync (context, 0, action, isPhoto, ref path, uri)
				.ContinueWith (t => t.Result.ToTask()).Unwrap();
		}
	}

	public sealed class MediaFile
		: IDisposable
	{
		internal MediaFile (string path, bool deletePathOnDispose)
		{
			this.deletePathOnDispose = deletePathOnDispose;
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

			return File.OpenRead (this.path);
		}

		public void Dispose()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		private bool isDisposed;
		private readonly bool deletePathOnDispose;
		private readonly string path;

		internal const string ExtraName = "MediaFile";
		
		private void Dispose (bool disposing)
		{
			if (this.isDisposed)
				return;

			this.isDisposed = true;
			if (this.deletePathOnDispose) {
				try {
					File.Delete (this.path);
					// We don't really care if this explodes for a normal IO reason.
				} catch (UnauthorizedAccessException) {
				} catch (DirectoryNotFoundException) {
				} catch (IOException) {
				}
			}
		}

		~MediaFile()
		{
			Dispose (false);
		}
	}
}