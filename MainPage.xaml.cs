using Microsoft.Maui.Controls;
using System;
using System.Timers;
using System.Threading.Tasks;

namespace WeatherApp
{
    public partial class MainPage : ContentPage
    {
        public WeatherViewModel ViewModel { get; }
        private System.Timers.Timer _timer;  // Use the fully qualified name

        public MainPage()
        {
            InitializeComponent();

            ViewModel = new WeatherViewModel();
            BindingContext = ViewModel;

            // Periodically update the weather data
            _timer = new System.Timers.Timer(60000); // Set the interval to 1 minute (60000 milliseconds)
            _timer.Elapsed += async (sender, e) => await UpdateWeatherData();
            _timer.Start();
        }

        private async Task UpdateWeatherData()
        {
            await ViewModel.GetCurrentLocation();
        }

        private void OnMenuClicked(object sender, EventArgs e)
        {
            MenuOverlay.IsVisible = true;
            MenuOverlay.TranslateTo(0, 0, 250, Easing.Linear);
        }

        private void OnCloseMenuClicked(object sender, EventArgs e)
        {
            MenuOverlay.TranslateTo(1000, 0, 250, Easing.Linear);
            MenuOverlay.IsVisible = false;
        }

        private async void OnOverlayGetWeatherClicked(object sender, EventArgs e)
        {
            string location = OverlayLocationEntry.Text;
            if (!string.IsNullOrEmpty(location))
            {
                await ViewModel.GetWeatherByLocationName(location);
                MenuOverlay.TranslateTo(1000, 0, 250, Easing.Linear);
                MenuOverlay.IsVisible = false;
            }
        }
    }
}
