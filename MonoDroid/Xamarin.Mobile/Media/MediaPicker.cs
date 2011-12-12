using System;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;

namespace Xamarin.Media
{
	public class MediaPicker
	{
		public MediaPicker (Context context)
		{
			this.context = context;
		}

		public Task<Photo> PickPhotoAsync()
		{
			int id = this.requestId;
			if (this.requestId == Int32.MaxValue)
				this.requestId = 0;
			else
				this.requestId++;

			Intent pickerIntent = new Intent (this.context, typeof (MediaPickerActivity));
			pickerIntent.PutExtra ("type", "image/*");
			pickerIntent.PutExtra ("id", id);
			this.context.StartActivity (pickerIntent);

			var tcs = new TaskCompletionSource<Photo> (id);
			EventHandler<MediaPickedEventArgs> handler = null;
			handler = (s, e) =>
			{
				MediaPickerActivity.MediaPicked -= handler;

				if (e.RequestId != id)
					return;

				if (e.Error != null)
					tcs.SetException (e.Error);
				else if (e.IsCanceled)
					tcs.SetCanceled();
				else
					tcs.SetResult ((Photo)e.Media);
			};

			MediaPickerActivity.MediaPicked += handler;

			return tcs.Task;
		}

		public Task<Video> PickVideoAsync()
		{
			throw new NotImplementedException();
		}

		private readonly Context context;
		private int requestId;
	}
}