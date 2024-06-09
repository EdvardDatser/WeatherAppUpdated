using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private string _humidity;
        private ObservableCollection<HourlyForecast> _hourlyForecasts;
        private string _wind_dir;
        private string _wind_kph;
        private string _feelslike_c;

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

        public string Humidity
        {
            get => _humidity;
            set
            {
                _humidity = value;
                OnPropertyChanged(nameof(Humidity));
            }
        }
        
        public string Wind_dir
        {
            get => _wind_dir;
            set
            {
                _wind_dir = value;
                OnPropertyChanged(nameof(Wind_dir));
            }
        }

        public string Wind_kph
        {
            get => _wind_kph;
            set
            {
                _wind_kph = value;
                OnPropertyChanged(nameof(Wind_kph));
            }
        }
        
        public string Feelslike_c
        {
            get => _feelslike_c;
            set
            {
                _feelslike_c = value;
                OnPropertyChanged(nameof(Feelslike_c));
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

        public ObservableCollection<HourlyForecast> HourlyForecasts
        {
            get => _hourlyForecasts;
            set
            {
                _hourlyForecasts = value;
                OnPropertyChanged(nameof(HourlyForecasts));
            }
        }

        public WeatherViewModel()
        {
            HourlyForecasts = new ObservableCollection<HourlyForecast>();
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
            string url = $"https://api.weatherapi.com/v1/forecast.json?key=f70e1b95550348eaab454224242905&q={locationName}&aqi=no&hours=24";

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
                        string wind_dir = data.current.wind_dir;
                        string wind_kph = data.current.wind_kph;
                        string feelslike_c = data.current.feelslike_c;

                        Location = curlocation;
                        Temperature = $"Temperature: {temperature}°C";
                        Condition = condition;

                        Wind_dir = $"Wind directiond: {wind_dir}";
                        Wind_kph = $"Wind speed: {wind_kph}km";
                        Feelslike_c = $"Feelslike: {feelslike_c}°C";

                        WeatherIconPath = GetWeatherIconPath(condition);

                        PopulateHourlyForecast(data);
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
            string url = $"https://api.weatherapi.com/v1/forecast.json?key=f70e1b95550348eaab454224242905&q={latitude},{longitude}&aqi=no&hours=24";

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
                        string humidity = data.current.humidity;
                        string wind_dir = data.current.wind_dir;
                        string wind_kph = data.current.wind_kph;
                        string feelslike_c = data.current.feelslike_c;

                        Location = curlocation;
                        Temperature = $"Temperature: {temperature}°C";
                        Condition = condition;

                        Wind_dir = $"Wind directiond: {wind_dir}";
                        Wind_kph = $"Wind speed: {wind_kph}km";
                        Feelslike_c = $"Feelslike: {feelslike_c}°C";

                        Humidity = $"Humidity: {humidity}%";
                        Location = curlocation;
                        Temperature = $"Temperature: {temperature}°C";
                        Condition = condition;
                        //уведомления
                        NotificationRequest request = new NotificationRequest
                        {
                            //Устанавливает уникальный идентификатор для уведомления
                            NotificationId = 1000,
                            Title = $"{Temperature}",
                            Subtitle = "Current weather",
                            Description = $"{Humidity}",
                            //Устанавливает номер значка (например, количество непрочитанных уведомлений).
                            BadgeNumber = 42,
                            //Создает расписание для уведомления
                            Schedule = new NotificationRequestSchedule
                            {
                                //Устанавливает время отправки уведомления на 5 секунд позже текущего времени
                                NotifyTime = DateTime.Now,
                                //Устанавливает интервал повторения уведомления каждые 10 секунд.
                                NotifyRepeatInterval = TimeSpan.FromMinutes(5),
                            },
                            Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions
                            {
                                //устанавливает приоритет уведомления на максимальный уровень.
                                Priority = Plugin.LocalNotification.AndroidOption.AndroidPriority.Max,
                            }
                        };
                        await LocalNotificationCenter.Current.Show(request);

                        WeatherIconPath = GetWeatherIconPath(condition);

                        PopulateHourlyForecast(data);
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

        private void PopulateHourlyForecast(dynamic data)
        {
            try
            {
                HourlyForecasts.Clear();

                var forecastData = data.forecast.forecastday[0].hour;

                // Find the index of the current hour
                int currentHourIndex = GetCurrentHourIndex(forecastData);

                // Start from the current hour index
                int startIndex = currentHourIndex >= 0 ? currentHourIndex : 0;

                for (int i = startIndex; i < forecastData.Count; i++)
                {
                    var hourData = forecastData[i];

                    string timeStr = hourData.time;
                    DateTime time = DateTime.Parse(timeStr);
                    string condition = hourData.condition.text;
                    string temperature = hourData.temp_c;

                    string iconPath = GetWeatherIconPath(condition);

                    HourlyForecasts.Add(new HourlyForecast
                    {
                        Time = time,
                        Icon = iconPath,
                        Temperature = temperature,
                        Condition = condition
                    });
                }
            }
            catch (Exception ex)
            {
                Error = $"An error occurred: {ex.Message}";
                    Debug.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        // Helper method to get the index of the current hour in the forecast data
        private int GetCurrentHourIndex(dynamic forecastData)
        {
            DateTime currentTime = DateTime.Now;
            string currentHourStr = currentTime.ToString("yyyy-MM-dd HH");

            for (int i = 0; i < forecastData.Count; i++)
            {
                string hourDataTimeStr = forecastData[i].time;
                if (hourDataTimeStr.StartsWith(currentHourStr))
                {
                    return i;
                }
            }

            return -1; // Current hour not found
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
