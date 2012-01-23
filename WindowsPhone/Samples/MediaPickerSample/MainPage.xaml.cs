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

				var bmp = new BitmapImage();
				bmp.SetSource (photo.GetStream());
				this.image.Source = bmp;

				if (photo.CanDelete)
					photo.Delete();
			}
			catch (TaskCanceledException ex)
			{
			}
		}

		private async void Take_OnClick (object sender, RoutedEventArgs e)
		{
			try
			{
				MediaFile photo = await picker.TakePhotoAsync (new StoreMediaOptions());

				var bmp = new BitmapImage();
				bmp.SetSource (photo.GetStream());
				this.image.Source = bmp;

				if (photo.CanDelete)
					photo.Delete();
			}
			catch (TaskCanceledException ex)
			{
			}
		}
	}
}