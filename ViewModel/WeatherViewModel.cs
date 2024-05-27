using Microsoft.Maui.Controls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace WeatherApp
{
    public class WeatherViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _location;
        private string _temperature;
        private string _condition;
        private string _weatherIconPath;
        private string _error;

        public string Error
        {
            get => _error;
            set
            {
                _error = value;
                OnPropertyChanged(nameof(Error));
            }
        }

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                OnPropertyChanged(nameof(Location));
            }
        }

        public string Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                OnPropertyChanged(nameof(Temperature));
            }
        }

        public string Condition
        {
            get => _condition;
            set
            {
                _condition = value;
                OnPropertyChanged(nameof(Condition));
            }
        }

        public string WeatherIconPath
        {
            get => _weatherIconPath;
            set
            {
                _weatherIconPath = value;
                OnPropertyChanged(nameof(WeatherIconPath));
            }
        }

        public WeatherViewModel()
        {
            // Call GetCurrentLocation at startup
            Task.Run(GetCurrentLocation);
        }

        public async Task GetCurrentLocation()
        {
            try
            {
                var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));

                if (location != null)
                {
                    double latitude = location.Latitude;
                    double longitude = location.Longitude;

                    await GetWeatherFromLocation(latitude, longitude);
                }
            }
            catch (Exception ex)
            {
                Error = $"Exception: {ex.Message}";
                Debug.WriteLine($"Exception: {ex.Message}");
            }
        }

        public async Task GetWeatherByLocationName(string locationName)
        {
            string url = $"https://api.weatherapi.com/v1/current.json?key=aa7758b35f384a5eb62102337241405&q={locationName}&aqi=no";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);

                        string curlocation = data.location.name;
                        string temperature = data.current.temp_c;
                        string condition = data.current.condition.text;

                        Location = curlocation;
                        Temperature = $"Temperature: {temperature}°C";
                        Condition = condition;

                        WeatherIconPath = GetWeatherIconPath(condition);
                    }
                    else
                    {
                        Error = $"Failed to retrieve weather data. Status code: {response.StatusCode}";
                        Debug.WriteLine($"Failed to retrieve weather data. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Error = $"An error occurred: {ex.Message}";
                    Debug.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        private async Task GetWeatherFromLocation(double latitude, double longitude)
        {
            string url = $"https://api.weatherapi.com/v1/current.json?key=aa7758b35f384a5eb62102337241405&q={latitude},{longitude}&aqi=no";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);

                        string curlocation = data.location.name;
                        string temperature = data.current.temp_c;
                        string condition = data.current.condition.text;

                        Location = curlocation;
                        Temperature = $"Temperature: {temperature}°C";
                        Condition = condition;

                        WeatherIconPath = GetWeatherIconPath(condition);
                    }
                    else
                    {
                        Error = $"Failed to retrieve weather data. Status code: {response.StatusCode}";
                        Debug.WriteLine($"Failed to retrieve weather data. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Error = $"An error occurred: {ex.Message}";
                    Debug.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        private string GetWeatherIconPath(string condition)
        {
            // Constructing the icon name based on the weather condition
            string iconName = "";

            if (condition.ToLower().Contains("rain"))
            {
                iconName = "rainy.svg";
            }
            else if (condition.ToLower().Contains("snow"))
            {
                iconName = "snowy.svg";
            }
            else if (condition.ToLower().Contains("sunny"))
            {
                iconName = "day.svg";
            }
            else if (condition.ToLower().Contains("cloud"))
            {
                iconName = "cloudy.svg";
            }
            else
            {
                iconName = "default.svg";
            }

            // Adjust the path according to your actual SVG file location
            return $"Resources.Images.{iconName}";
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
