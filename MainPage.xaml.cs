﻿using Microsoft.Maui.Controls;
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
                await UpdateWeatherData();
                ErrorLabel.Text = null;
            }
            else
            {
                var permissionStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (permissionStatus == PermissionStatus.Granted)
                {
                    await UpdateWeatherData();
                    ErrorLabel.Text = null;
                }
                else
                {
                    ErrorLabel.Text = "Location permission denied.";
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            WeatherUpdate();
        }

        private void WeatherUpdate()
        {
            if (SelectFavoriteCity)
            {
                ViewModel.GetWeatherByLocationName(anotherlocation);
                SelectFavoriteCity = false;
                var navigationStack = Navigation.NavigationStack;

                // Удаление Page1 и Page2 из стека
                for (int i = navigationStack.Count - 2; i >= 0; i--) // - 2 это предпоследняя страница; если ставить - 1 будет последняя
                {
                    //Внутри цикла мы получаем ссылку на страницу по текущему индексу i и вызываем метод RemovePage у объекта Navigation, чтобы удалить эту страницу из стека навигации.
                    var page = navigationStack[i];
                    Navigation.RemovePage(page);
                }
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
