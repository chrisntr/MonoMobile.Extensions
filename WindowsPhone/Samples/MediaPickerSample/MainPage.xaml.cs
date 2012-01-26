using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Xamarin.Media;

namespace MediaPickerSample
{
	public partial class MainPage : PhoneApplicationPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private readonly MediaPicker picker = new MediaPicker();

		private async void Pick_OnClick (object sender, RoutedEventArgs e)
		{
			try
			{
				MediaFile photo = await picker.PickPhotoAsync();
				this.image.Source = new BitmapImage (new Uri (photo.Path));
			}
			catch (TaskCanceledException ex)
			{
			}
		}

		private async void Take_OnClick (object sender, RoutedEventArgs e)
		{
			try
			{
				MediaFile photo = await picker.TakePhotoAsync (new StoreCameraMediaOptions());
				var source = new BitmapImage();
				using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
					source.SetSource (storage.OpenFile (photo.Path, FileMode.Open));

				this.image.Source = source;
			}
			catch (TaskCanceledException ex)
			{
			}
		}
	}
}