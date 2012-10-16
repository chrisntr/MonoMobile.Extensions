using System;
using System.IO;

namespace Xamarin.Media
{
	internal static class MediaExtensions
	{
		internal static void VerifyOptions (this StoreMediaOptions self)
		{
			if (self == null)
				throw new ArgumentNullException ("options");
			//if (!Enum.IsDefined (typeof(MediaFileStoreLocation), options.Location))
			//    throw new ArgumentException ("options.Location is not a member of MediaFileStoreLocation");
			//if (options.Location == MediaFileStoreLocation.Local)
			//{
				//if (String.IsNullOrWhiteSpace (options.Directory))
				//	throw new ArgumentNullException ("options", "For local storage, options.Directory must be set");
				if (Path.IsPathRooted (self.Directory))
					throw new ArgumentException ("options.Directory must be a relative path", "options");
			//}
		}

		internal static string GetFilePath (this StoreMediaOptions self, string rootPath)
		{
			bool isPhoto = !(self is StoreVideoOptions);

			string name = (self != null) ? self.Name : null;
			if (String.IsNullOrWhiteSpace (name))
			{
				string timestamp = DateTime.Now.ToString ("yyyyMMdd_HHmmss");
				if (isPhoto)
					name = "IMG_" + timestamp + ".jpg";
				else
					name = "VID_" + timestamp + ".mp4";
			}

			string ext = Path.GetExtension (name);
			if (ext == String.Empty)
				ext = ((isPhoto) ? ".jpg" : ".mp4");

			name = Path.GetFileNameWithoutExtension (name);

			string folder = Path.Combine (rootPath ?? String.Empty,
				(self != null && self.Directory != null) ? self.Directory : String.Empty);

			return Path.Combine (folder, name + ext);
		}

		internal static string GetUniqueFilepath (this StoreMediaOptions self, string rootPath, Func<string, bool> checkExists)
		{
			string path = self.GetFilePath (rootPath);
			string folder = Path.GetDirectoryName (path);
			string ext = Path.GetExtension (path);
			string name = Path.GetFileNameWithoutExtension (path);

			string nname = name + ext;
			int i = 1;
			while (checkExists (Path.Combine (folder, nname)))
				nname = name + "_" + (i++) + ext;

			return Path.Combine (folder, nname);
		}
	}
}