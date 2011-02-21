using System.Windows;
using MonoMobile.Extensions;

namespace MonoMobile.Example
{
    public partial class Geolocation
    {
        private readonly Extensions.Geolocation _geolocation;
        private readonly GeolocationOptions _options;
        private string _watchId;

        public Geolocation()
        {
            InitializeComponent();

            _geolocation = new Extensions.Geolocation();

            _options = new GeolocationOptions();
            _options.EnableHighAccuracy = true;

            _getSingle.Click += _getSingle_Click;
            _start.Click += _start_Click;
            _stop.Click += _stop_Click;
        }

        private void _getSingle_Click(object sender, RoutedEventArgs e)
        {
            _geolocation.GetCurrentPosition(OnSuccess, OnError, _options);
        }

        private void _start_Click(object sender, RoutedEventArgs e)
        {
            _watchId = _geolocation.WatchPosition(OnSuccess, OnError, _options);
        }

        private void _stop_Click(object sender, RoutedEventArgs e)
        {
            _geolocation.ClearWatch(_watchId);
        }

        private void OnSuccess(Position position)
        {
            _output.Text += string.Format("{0}, {1}\r\n", position.Coords.Latitude, position.Coords.Longitude);
        }

        private void OnError(PositionError error)
        {
            _output.Text += string.Format("{0}: {1}\r\n", error.Code, error.Message);
        }
    }
}