using Microsoft.Maui.Controls;
using System;
using System.Timers;
using System.Threading.Tasks;
using WeatherApp.ViewModel;

namespace WeatherApp
{
    public partial class MainPage : ContentPage
    {
        public WeatherViewModel ViewModel { get; }
        private System.Timers.Timer _timer;
        MainViewModel mainViewModel = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();

            ViewModel = new WeatherViewModel();
            BindingContext = ViewModel;

            // Periodically update the weather data
            _timer = new System.Timers.Timer(900000); // Set the interval to 15 minutes (900000 milliseconds)
            _timer.Elapsed += async (sender, e) => await UpdateWeatherData();
            _timer.Start();

            GetPermission();
        }

        public string anotherlocation;
        public bool SelectFavoriteCity = false;

        bool permission = false;
        public async void GetPermission()
        {
            var status = PermissionStatus.Unknown;
            status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                return;
            }
            else
            {
                await mainViewModel.RequsetLocation();
                await UpdateWeatherData();
                ErrorLabel.Text = null;
            }

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            WeatherUpdate();
        }

        // Change protection level to public or internal
        public void WeatherUpdate()
        {
            if (SelectFavoriteCity)
            {
                ViewModel.GetWeatherByLocationName(anotherlocation);
                SelectFavoriteCity = false;
                Shell.SetNavBarIsVisible(this, false);
            }
        }

        private async void OnFavoriteClicked(object sender, EventArgs e)
        {
            DBListPage listPage = new DBListPage();
            await Navigation.PushAsync(listPage);
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
