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

		private Uri path;
		private bool isPhoto;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			int id = this.Intent.GetIntExtra (ExtraId, 0);
			string type = this.Intent.GetStringExtra (ExtraType);
			if (type == "image/*")
				this.isPhoto = true;

			string action = this.Intent.GetStringExtra (ExtraAction);

			Intent pickIntent = null;
			try
			{
				pickIntent = new Intent (action);
				if (action == Intent.ActionPick)
					pickIntent.SetType (type);
				else
				{
					if (!this.isPhoto)
					{
						int seconds = this.Intent.GetIntExtra (MediaStore.ExtraDurationLimit, 0);
						if (seconds != 0)
							pickIntent.PutExtra (MediaStore.ExtraDurationLimit, seconds);
					}

					pickIntent.PutExtra (MediaStore.ExtraVideoQuality, this.Intent.GetIntExtra (MediaStore.ExtraVideoQuality, 1));

					MediaFileStoreLocation loc = (MediaFileStoreLocation) this.Intent.GetIntExtra (ExtraLocation, 0);
					this.path = GetOutputMediaFile (loc, this.Intent.GetStringExtra (ExtraPath), this.Intent.GetStringExtra (MediaStore.MediaColumns.Title));
					Touch();

					pickIntent.PutExtra (MediaStore.ExtraOutput, this.path);
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
				Uri targetUri = (data != null) ? data.Data : this.path;
				if (targetUri == null)
					targetUri = this.path;

				var mf = new MediaFile (() => GetStreamForUri (targetUri), () => DeleteFileAtUri (targetUri));
				args = new MediaPickedEventArgs (requestCode, false, mf);
			}

			OnMediaPicked (args);
			Finish();
		}

		private string GetUniquePath (string folder, string name)
		{
			string ext = Path.GetExtension (name) ?? ((this.isPhoto) ? ".jpg" : ".mp4");
			name = Path.GetFileNameWithoutExtension (name);

			string nname = name + ext;
			int i = 1;
			while (File.Exists (Path.Combine (folder, nname)))
				nname = name + "_" + (i++) + ext;

			return Path.Combine (folder, nname);
		}

		private Uri GetOutputMediaFile (MediaFileStoreLocation loc, string subdir, string name)
		{
			if (loc == MediaFileStoreLocation.None && subdir == null)
				subdir = "tmp";

			string type = (this.isPhoto) ? Environment.DirectoryPictures : Environment.DirectoryMovies;
			Java.IO.File mediaStorageDir = new Java.IO.File (GetExternalFilesDir (type), subdir);
			if (!mediaStorageDir.Exists())
			{
				if (!mediaStorageDir.Mkdirs())
					return null;
			}

			if (String.IsNullOrWhiteSpace (name))
			{
				string timestamp = DateTime.Now.ToString ("yyyyMMdd_HHmmss");
				if (this.isPhoto)
					name = "IMG_" + timestamp + ".jpg";
				else
					name = "VID_" + timestamp + ".mp4";
			}

			return Uri.FromFile (new Java.IO.File (GetUniquePath (mediaStorageDir.Path, name)));
		}

		private Stream GetStreamForUri (Uri uri)
		{
			if (uri.Scheme == "file")
				return File.OpenRead (new System.Uri (uri.ToString()).LocalPath);

			ICursor c = null;
			try
			{
				c = ContentResolver.Query (uri, null, null, null, null);
				if (!c.MoveToNext())
					return null;

				byte[] image = c.GetBlob (c.GetColumnIndex (MediaStore.MediaColumns.Data));
				return new MemoryStream (image);
				/*string path = c.GetString (c.GetColumnIndex (MediaStore.MediaColumns.Title));
				bool exists = File.Exists (path);//Path.Combine (Environment.DirectoryPictures, path));
				exists.ToString();*/
			}
			finally
			{
				if (c != null)
					c.Close();
			}
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