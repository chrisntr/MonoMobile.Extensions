namespace Xamarin.Media
{
	public class MediaFile
	{
		internal MediaFile (string path)
		{
			Path = path;
		}

		public string Path
		{
			get;
			private set;
		}
	}
}