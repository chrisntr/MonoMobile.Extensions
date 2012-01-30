using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Environment = Android.OS.Environment;
using Path = System.IO.Path;
using Uri = Android.Net.Uri;

namespace Xamarin.Media
{
	[Activity]
	internal class MediaPickerActivity
		: Activity
	{
		internal const string ExtraPath = "path";
		internal const string ExtraLocation = "location";
		internal const string ExtraType = "type";
		internal const string ExtraId = "id";
		internal const string ExtraAction = "action";

		internal static event EventHandler<MediaPickedEventArgs> MediaPicked;

		private string title;
		private string description;
		private Uri path;
		private bool isPhoto;
		private string action;
		//private MediaFileStoreLocation location;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			this.title = this.Intent.GetStringExtra (MediaStore.MediaColumns.Title);
			this.description = this.Intent.GetStringExtra (MediaStore.Images.ImageColumns.Description);

			int id = this.Intent.GetIntExtra (ExtraId, 0);
			string type = this.Intent.GetStringExtra (ExtraType);
			if (type == "image/*")
				this.isPhoto = true;

			this.action = this.Intent.GetStringExtra (ExtraAction);
			Intent pickIntent = null;
			try
			{
				pickIntent = new Intent (this.action);
				if (action == Intent.ActionPick)
				{
					//this.location = MediaFileStoreLocation.CameraRoll;
					pickIntent.SetType (type);
				}
				else
				{
					if (!this.isPhoto)
					{
						int seconds = this.Intent.GetIntExtra (MediaStore.ExtraDurationLimit, 0);
						if (seconds != 0)
							pickIntent.PutExtra (MediaStore.ExtraDurationLimit, seconds);
					}

					VideoQuality quality = (VideoQuality)this.Intent.GetIntExtra (MediaStore.ExtraVideoQuality, (int)VideoQuality.High);
					pickIntent.PutExtra (MediaStore.ExtraVideoQuality, GetVideoQuality (quality));

					//this.location = (MediaFileStoreLocation) this.Intent.GetIntExtra (ExtraLocation, 0);
					//this.path = GetOutputMediaFile (this.location, this.Intent.GetStringExtra (ExtraPath), this.title, this.description);
					this.path = GetOutputMediaFile (this.Intent.GetStringExtra (ExtraPath), this.title, this.description);

					if (this.isPhoto)
					{
						Touch();
						pickIntent.PutExtra (MediaStore.ExtraOutput, this.path);
					}
				}

				StartActivityForResult (pickIntent, id);
			}
			catch (Exception ex)
			{
				OnMediaPicked (new MediaPickedEventArgs (id, ex));
			}
			finally
			{ 
				if (pickIntent != null)
					pickIntent.Dispose();
			}
		}
		
		private void Touch()
		{
			if (this.path.Scheme != "file")
				return;

			File.Create (GetLocalPath (this.path)).Close();
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			MediaPickedEventArgs args;
			if (resultCode == Result.Canceled)
				args = new MediaPickedEventArgs (requestCode, isCanceled: true);
			else
			{
				string filePath = null;
				//if (this.location == MediaFileStoreLocation.Local)
				//{
				if (this.action != Intent.ActionPick)
				{
					if (data != null && data.Data != null)
						MoveFile (data.Data);

					filePath = this.path.Path;
				}
				else
				{
				    if (data != null && data.Data != null)
						this.path = data.Data;

				    filePath = GetFilePathForUri (this.path);

				//    ContentValues values = new ContentValues();
				//    if (this.isPhoto)
				//    {
				//        values.Put (MediaStore.Images.ImageColumns.Title, this.title);
				//        values.Put (MediaStore.Images.ImageColumns.Description, this.description);
				//        values.Put (MediaStore.Images.ImageColumns.DateTaken, DateTime.Now.ToAndroidTimestamp());
				//        values.Put (MediaStore.MediaColumns.Data, filePath);
				//        ContentResolver.Insert (MediaStore.Images.Media.ExternalContentUri, values);
				//    }
				//    else
				//    {
				//        values.Put (MediaStore.Video.VideoColumns.Title, this.title);
				//        values.Put (MediaStore.Video.VideoColumns.Description, this.description);
				//        ContentResolver.Update (this.path, values, null, null);
				//    }
				}

				var mf = new MediaFile (filePath, () => File.OpenRead (filePath));
				args = new MediaPickedEventArgs (requestCode, false, mf);
			}

			OnMediaPicked (args);
			Finish();
		}
		
		private void MoveFile (Uri url)
		{
			ICursor cursor = null;
			try
			{
	            cursor = ContentResolver.Query (url, null, null, null, null);
				if (cursor.MoveToFirst())
				{
					String filename = cursor.GetString (cursor.GetColumnIndex (MediaStore.Video.Media.InterfaceConsts.Data));
					File.Move (filename, this.path.EncodedPath);
					ContentResolver.Delete (url, null, null);
				}
			}
			finally
			{
				if (cursor != null)
					cursor.Close();
			}
		}

		private int GetVideoQuality (VideoQuality quality)
		{
			switch (quality)
			{
				case VideoQuality.Medium:
				case VideoQuality.High:
					return 1;

				default:
					return 0;
			}
		}

		private string GetUniquePath (string folder, string name)
		{
			string ext = Path.GetExtension (name);
			if (ext == String.Empty)
				ext = ((this.isPhoto) ? ".jpg" : ".mp4");

			name = Path.GetFileNameWithoutExtension (name);

			string nname = name + ext;
			int i = 1;
			while (File.Exists (Path.Combine (folder, nname)))
				nname = name + "_" + (i++) + ext;

			return Path.Combine (folder, nname);
		}

		private Uri GetOutputMediaFile (string subdir, string name, string description)
		{
			subdir = subdir ?? String.Empty;

			if (String.IsNullOrWhiteSpace (name))
			{
				string timestamp = DateTime.Now.ToString ("yyyyMMdd_HHmmss");
				if (this.isPhoto)
					name = "IMG_" + timestamp + ".jpg";
				else
					name = "VID_" + timestamp + ".mp4";
			}

			//if (loc == MediaFileStoreLocation.CameraRoll)
			//{
			//    ContentValues values = new ContentValues();
			//    if (this.isPhoto)
			//    {
			//        values.Put (MediaStore.Images.ImageColumns.Title, name);
			//        values.Put (MediaStore.Images.ImageColumns.Description, description);
			//        values.Put (MediaStore.Images.ImageColumns.DateTaken, DateTime.Now.ToAndroidTimestamp());
			//        return ContentResolver.Insert (MediaStore.Images.Media.ExternalContentUri, values);
			//    }
			//    else
			//    {
			//        values.Put (MediaStore.Video.VideoColumns.Title, name);
			//        values.Put (MediaStore.Video.VideoColumns.Description, description);
			//        values.Put (MediaStore.Video.VideoColumns.DateTaken, DateTime.Now.ToAndroidTimestamp());
			//        return ContentResolver.Insert (MediaStore.Video.Media.ExternalContentUri, values);
			//    }
			//}
			//else
			//{
				string type = (this.isPhoto) ? Environment.DirectoryPictures : Environment.DirectoryMovies;
				Java.IO.File mediaStorageDir = new Java.IO.File (GetExternalFilesDir (type), subdir);
				if (!mediaStorageDir.Exists())
				{
					if (!mediaStorageDir.Mkdirs())
						return null;

					Java.IO.File nomedia = new Java.IO.File (mediaStorageDir, ".nomedia");
					nomedia.CreateNewFile();
				}

				return Uri.FromFile (new Java.IO.File (GetUniquePath (mediaStorageDir.Path, name)));
			//}
		}

		private string GetFilePathForUri (Uri uri)
		{
			if (uri.Scheme == "file")
				return new System.Uri (uri.ToString()).LocalPath;

			ICursor c = null;
			try
			{
				c = ContentResolver.Query (uri, null, null, null, null);
				if (!c.MoveToNext())
					return null;

				return c.GetString (c.GetColumnIndex (MediaStore.MediaColumns.Data));
			}
			finally
			{
				if (c != null)
					c.Close();
			}
		}

		private Stream GetStreamForUri (Uri uri)
		{
			return File.OpenRead (GetFilePathForUri (uri));
		}

		private string GetLocalPath (Uri uri)
		{
			return new System.Uri (uri.ToString()).LocalPath;
		}

		private void DeleteFileAtUri (Uri uri)
		{
			if (uri.Scheme == "file")
				File.Delete (GetLocalPath (uri));
			else
				ContentResolver.Delete (uri, null, null);
		}

		private static void OnMediaPicked (MediaPickedEventArgs e)
		{
			var picked = MediaPicked;
			if (picked != null)
				picked (null, e);
		}
	}

	internal class MediaPickedEventArgs
		: EventArgs
	{
		public MediaPickedEventArgs (int id, Exception error)
		{
			if (error == null)
				throw new ArgumentNullException ("error");

			RequestId = id;
			Error = error;
		}

		public MediaPickedEventArgs (int id, bool isCanceled, MediaFile media = null)
		{
			RequestId = id;
			IsCanceled = isCanceled;
			if (!IsCanceled && media == null)
				throw new ArgumentNullException ("media");

			Media = media;
		}

		public int RequestId
		{
			get;
			private set;
		}

		public bool IsCanceled
		{
			get;
			private set;
		}

		public Exception Error
		{
			get;
			private set;
		}

		public MediaFile Media
		{
			get;
			private set;
		}
	}
}