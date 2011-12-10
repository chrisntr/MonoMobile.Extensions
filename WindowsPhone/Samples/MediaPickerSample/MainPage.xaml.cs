using System.Windows;
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

		private MediaPicker picker = new MediaPicker();
		private async void button1_Click(object sender, RoutedEventArgs e)
		{
			Photo photo = await picker.PickPhotoAsync();

		}
	}
}