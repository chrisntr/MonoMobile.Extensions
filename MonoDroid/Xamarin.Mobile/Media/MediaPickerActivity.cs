using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

		private int id;
		private string title;
		private string description;
		private string type;

		/// <summary>
		/// The user's destination path.
		/// </summary>
		private Uri path;
		private bool isPhoto;
		private string action;

		private int seconds;
		private VideoQuality quality;

		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutBoolean ("ran", true);
			outState.PutString (MediaStore.MediaColumns.Title, this.title);
			outState.PutString (MediaStore.Images.ImageColumns.Description, this.description);
			outState.PutInt (ExtraId, this.id);
			outState.PutString (ExtraType, this.type);
			outState.PutString (ExtraAction, this.action);
			outState.PutInt (MediaStore.ExtraDurationLimit, this.seconds);
			outState.PutInt (MediaStore.ExtraVideoQuality, (int)this.quality);

			if (this.path != null)
				outState.PutString (ExtraPath, this.path.Path);

			base.OnSaveInstanceState (outState);
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			Bundle b = (savedInstanceState ?? Intent.Extras);

			bool ran = b.GetBoolean ("ran", defaultValue: false);

			this.title = b.GetString (MediaStore.MediaColumns.Title);
			this.description = b.GetString (MediaStore.Images.ImageColumns.Description);

			this.id = b.GetInt (ExtraId, 0);
			this.type = b.GetString (ExtraType);
			if (this.type == "image/*")
				this.isPhoto = true;

			this.action = b.GetString (ExtraAction);
			Intent pickIntent = null;
			try
			{
				pickIntent = new Intent (this.action);
				if (this.action == Intent.ActionPick)
					pickIntent.SetType (type);
				else
				{
					if (!this.isPhoto)
					{
						this.seconds = b.GetInt (MediaStore.ExtraDurationLimit, 0);
						if (this.seconds != 0)
							pickIntent.PutExtra (MediaStore.ExtraDurationLimit, seconds);
					}

					this.quality = (VideoQuality)b.GetInt (MediaStore.ExtraVideoQuality, (int)VideoQuality.High);
					pickIntent.PutExtra (MediaStore.ExtraVideoQuality, GetVideoQuality (this.quality));

					if (!ran)
						this.path = GetOutputMediaFile (b.GetString (ExtraPath), this.title);
					else
						this.path = Uri.Parse (b.GetString (ExtraPath));

					if (this.isPhoto && !ran)
					{
						Touch();
						pickIntent.PutExtra (MediaStore.ExtraOutput, this.path);
					}
				}

				if (!ran)
					StartActivityForResult (pickIntent, this.id);
			}
			catch (Exception ex)
			{
				OnMediaPicked (new MediaPickedEventArgs (this.id, ex));
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

			Task<MediaPickedEventArgs> future;

			if (resultCode == Result.Canceled)
				future = TaskFromResult (new MediaPickedEventArgs (requestCode, isCanceled: true));
			else
			{
				Task<Tuple<string, bool>> pathFuture;

				string originalPath = null;

				if (this.action != Intent.ActionPick)
				{
					originalPath = this.path.Path;

					// Not all camera apps respect EXTRA_OUTPUT, some will instead
					// return a content or file uri from data.
					if (data != null && data.Data != null)
					{
						originalPath = data.DataString;
						pathFuture = TryMoveFileAsync (data.Data).ContinueWith (t =>
							new Tuple<string, bool> (t.Result ? this.path.Path : null, false));
					}
					else
						pathFuture = TaskFromResult (new Tuple<string, bool> (this.path.Path, false));
				}
				else if (data != null && data.Data != null)
				{
					originalPath = data.DataString;
					this.path = data.Data;
					pathFuture = GetFileForUriAsync (this.path);
				}
				else
					pathFuture = TaskFromResult<Tuple<string, bool>> (null);

				future = pathFuture.ContinueWith (t =>
				{
					string resultPath = t.Result.Item1;
					if (resultPath != null && File.Exists (t.Result.Item1))
					{
						Action<bool> dispose = null;
						if (t.Result.Item2)
							dispose = d => File.Delete (resultPath);

						var mf = new MediaFile (t.Result.Item1, () => File.OpenRead (resultPath), dispose);
						return new MediaPickedEventArgs (requestCode, false, mf);
					}
					else
						return new MediaPickedEventArgs (requestCode, new MediaFileNotFoundException (originalPath));
				});
			}

			Finish();

			future.ContinueWith (t => OnMediaPicked (t.Result));
		}
		
		private Task<bool> TryMoveFileAsync (Uri url)
		{
			string moveTo = GetLocalPath (this.path);
			return GetFileForUriAsync (url).ContinueWith (t =>
			{
				if (t.Result.Item1 == null)
					return false;

				File.Delete (moveTo);
				File.Move (t.Result.Item1, moveTo);

				if (url.Scheme == "content")
					ContentResolver.Delete (url, null, null);

				return true;
			});
		}

		private int GetVideoQuality (VideoQuality videoQuality)
		{
			switch (videoQuality)
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

		private Uri GetOutputMediaFile (string subdir, string name)
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

			string mediaType = (this.isPhoto) ? Environment.DirectoryPictures : Environment.DirectoryMovies;
			using (Java.IO.File mediaStorageDir = new Java.IO.File (GetExternalFilesDir (mediaType), subdir))
			{
				if (!mediaStorageDir.Exists())
				{
					if (!mediaStorageDir.Mkdirs())
						throw new IOException ("Couldn't create directory, have you added the WRITE_EXTERNAL_STORAGE permission?");

					// Ensure this media doesn't show up in gallery apps
					using (Java.IO.File nomedia = new Java.IO.File (mediaStorageDir, ".nomedia"))
						nomedia.CreateNewFile();
				}

				return Uri.FromFile (new Java.IO.File (GetUniquePath (mediaStorageDir.Path, name)));
			}
		}

		private Task<Tuple<string, bool>> GetFileForUriAsync (Uri uri)
		{
			var tcs = new TaskCompletionSource<Tuple<string, bool>>();

			if (uri.Scheme == "file")
				tcs.SetResult (new Tuple<string, bool> (new System.Uri (uri.ToString()).LocalPath, false));
			else if (uri.Scheme == "content")
			{
				Task.Factory.StartNew (() =>
				{
					ICursor cursor = null;
					try
					{
						cursor = ContentResolver.Query (uri, null, null, null, null);
						if (cursor == null || !cursor.MoveToNext())
							tcs.SetResult (new Tuple<string, bool> (null, false));
						else
						{
							int column = cursor.GetColumnIndex (MediaStore.MediaColumns.Data);
							string contentPath = null;

							if (column != -1)
								contentPath = cursor.GetString (column);

							bool copied = false;

							// If they don't follow the "rules", try to copy the file locally
							if (contentPath == null || !contentPath.StartsWith ("file"))
							{
								copied = true;
								Uri outputPath = GetOutputMediaFile ("temp", null);

								try
								{
									using (Stream input = ContentResolver.OpenInputStream (uri))
									using (Stream output = File.Create (outputPath.Path))
										input.CopyTo (output);

									contentPath = outputPath.Path;
								}
								catch (Java.IO.FileNotFoundException)
								{
									// If there's no data associated with the uri, we don't know
									// how to open this. contentPath will be null which will trigger
									// MediaFileNotFoundException.
								}
							}

							tcs.SetResult (new Tuple<string, bool> (contentPath, copied));
						}
					}
					finally
					{
						if (cursor != null)
						{
							cursor.Close();
							cursor.Dispose();
						}
					}
				}, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
			}
			else
				tcs.SetResult (new Tuple<string, bool> (null, false));

			return tcs.Task;
		}

		private string GetLocalPath (Uri uri)
		{
			return new System.Uri (uri.ToString()).LocalPath;
		}

		private static Task<T> TaskFromResult<T> (T result)
		{
			var tcs = new TaskCompletionSource<T>();
			tcs.SetResult (result);
			return tcs.Task;
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