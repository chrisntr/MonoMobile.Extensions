using System;

namespace Xamarin.Media
{
	public class Media
	{
		protected Media (string path)
		{
			if (path == null)
				throw new ArgumentNullException ("path");

			FilePath = path;
		}

		public string FilePath
		{
			get;
			private set;
		}
	}
}