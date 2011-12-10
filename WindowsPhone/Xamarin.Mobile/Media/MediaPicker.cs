using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Phone.Tasks;

namespace Xamarin.Media
{
	public class MediaPicker
	{
		public MediaPicker()
		{
			this.photoChooser.Completed += OnPhotoChosen;
			ShowCamera = true;
		}

		public bool ShowCamera
		{
			get { return this.photoChooser.ShowCamera; }
			set { this.photoChooser.ShowCamera = value; }
		}

		public bool PhotosSupported
		{
			get { return true; }
		}

		public bool VideosSupported
		{
			get { return false; }
		}

		public Task<Photo> PickPhotoAsync()
		{
			var ntcs = new TaskCompletionSource<Photo>();
			var tcs = Interlocked.CompareExchange (ref this.completionSource, ntcs, null);
			if (tcs != null)
				throw new InvalidOperationException ("Only one pick operation can be active at at time");

			this.photoChooser.Show();

			return ntcs.Task;
		}

		public Task<Video> PickVideoAsync()
		{
			throw new NotSupportedException();
		}

		private readonly PhotoChooserTask photoChooser = new PhotoChooserTask();
		private TaskCompletionSource<Photo> completionSource;

		private void OnPhotoChosen (object sender, PhotoResult photoResult)
		{
			var tcs = Interlocked.Exchange (ref this.completionSource, null);

			switch (photoResult.TaskResult)
			{
				case TaskResult.OK:
					tcs.SetResult (new Photo (photoResult.OriginalFileName));
					break;

				case TaskResult.None:
					if (photoResult.Error != null)
						tcs.SetException (photoResult.Error);

					break;

				case TaskResult.Cancel:
					tcs.SetCanceled();
					break;
			}
		}
	}
}