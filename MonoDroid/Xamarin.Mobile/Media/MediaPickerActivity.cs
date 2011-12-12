using System;
using Android.App;
using Android.Content;
using Android.OS;
using Uri = Android.Net.Uri;

namespace Xamarin.Media
{
	[Activity]
	internal class MediaPickerActivity
		: Activity
	{
		internal static event EventHandler<MediaPickedEventArgs> MediaPicked;

		private bool isPhoto;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			int id = this.Intent.GetIntExtra ("id", 0);
			string type = this.Intent.GetStringExtra ("type");
			if (type == "image/*")
				isPhoto = true;

			Intent pickIntent = null;
			try
			{
				pickIntent = new Intent (Intent.ActionPick);
				pickIntent.SetType (type);
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

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			MediaPickedEventArgs args;
			if (resultCode == Result.Canceled)
				args = new MediaPickedEventArgs (requestCode, isCanceled: true);
			else
			{
				string targetUri = data.Data.ToString();
				object media;
				if (isPhoto)
					media = new Photo (targetUri);
				else
					media = new Video (targetUri);

				args = new MediaPickedEventArgs (requestCode, false, media);
			}

			OnMediaPicked (args);
			Finish();
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

		public MediaPickedEventArgs (int id, bool isCanceled, object media = null)
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

		public object Media
		{
			get;
			private set;
		}
	}
}