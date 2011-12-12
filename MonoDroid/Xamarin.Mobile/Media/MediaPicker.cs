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
			return PickMediaAsync<Photo> ("image/*");
		}

		public Task<Video> PickVideoAsync()
		{
			return PickMediaAsync<Video> ("video/*");
		}

		private readonly Context context;
		private int requestId;

		private Task<T> PickMediaAsync<T> (string type)
			where T : Media
		{
			int id = this.requestId;
			if (this.requestId == Int32.MaxValue)
				this.requestId = 0;
			else
				this.requestId++;

			Intent pickerIntent = new Intent (this.context, typeof (MediaPickerActivity));
			pickerIntent.PutExtra ("type", type);
			pickerIntent.PutExtra ("id", id);
			this.context.StartActivity (pickerIntent);

			var tcs = new TaskCompletionSource<T> (id);
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
					tcs.SetResult ((T)e.Media);
			};

			MediaPickerActivity.MediaPicked += handler;

			return tcs.Task;
		}
	}
}