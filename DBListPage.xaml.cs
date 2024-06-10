using Newtonsoft.Json.Linq;
using WeatherApp.ViewModel;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace WeatherApp;

public partial class DBListPage : ContentPage
{
    MainPage mainPage = new MainPage();
    public ObservableCollection<City> Cities { get; set; }

    public DBListPage()
    {
        InitializeComponent();
        Cities = new ObservableCollection<City>(App.Database.GetItems());
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateWeather();
    }

    private async void UpdateWeather()
    {
        // Get list of cities from the database
        var cities = App.Database.GetItems();

        // Update temperature for each city
        foreach (var city in cities)
        {
            await RetrieveWeatherData(city.CityName, city);
        }

        // Update data source for the list of cities
        Cities.Clear();
        foreach (var city in cities)
        {
            Cities.Add(city);
        }
    }

    private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        City selectedCity = (City)e.SelectedItem;

        mainPage.anotherlocation = selectedCity.CityName;

        City selectCity = App.Database.SelectCityByName(mainPage.anotherlocation);

        if (selectCity != null)
        {
            mainPage.SelectFavoriteCity = true;
            await Navigation.PushAsync(mainPage);
        }
        else
        {
            await DisplayAlert("Error", "City not found", "OK");
        }
    }

    public async void AddFavorite(object sender, EventArgs e)
    {
        City city = new City();
        DBpage dBpage = new DBpage();
        dBpage.BindingContext = city;
        await Navigation.PushAsync(dBpage);
    }

    private async void MuudaCity(object sender, EventArgs e)
    {
        City selectedCity = (City)((MenuItem)sender).BindingContext;
        DBpage dBpage = new DBpage();
        dBpage.BindingContext = selectedCity;
        await Navigation.PushAsync(dBpage);
    }

    private void DeleteCity(object sender, EventArgs e)
    {
        var city = (City)((MenuItem)sender).BindingContext;
        App.Database.DeleteItem(city.Id);
        UpdateWeather();
    }

    public async Task RetrieveWeatherData(string location, City city)
    {
        string url = $"https://api.weatherapi.com/v1/current.json?key=f70e1b95550348eaab454224242905&q={location}&aqi=no";

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(jsonResponse);
                string temperature = data["current"]["temp_c"].ToString();
                string humidity = data["current"]["humidity"].ToString();
                string condition = data["current"]["condition"]["text"].ToString();

                // Set temperature, humidity, and condition properties
                city.temperature = temperature;
                city.humidity = humidity;
                city.condition = condition;

                // Set WeatherIconPath based on condition
                city.WeatherIconPath = GetWeatherIcon(condition);

                // Save the updated city data
                App.Database.SaveItem(city);
            }
            else
            {
                await DisplayAlert("Error", "City not found. Check the spelling of the city", "OK");
            }
        }
    }

    public static string GetWeatherIcon(string condition)
    {
        string basePath = "Wicons/";
        string iconPath = basePath + "default.svg"; // Default icon

        try
        {
            if (condition.Contains("Clear") || condition.Contains("Sunny"))
            {
                iconPath = basePath + "sun.png";
            }
            else if (condition.Contains("Cloudy") || condition.Contains("Overcast") || condition.Contains("Partly cloudy"))
            {
                iconPath = basePath + "cloud.png";
            }
            else if (condition.Contains("Mist") || condition.Contains("Fog"))
            {
                iconPath = basePath + "fog.png";
            }
            else if (condition.Contains("Rain") || condition.Contains("rain") || condition.Contains("Light") || condition.Contains("Patchy"))
            {
                iconPath = basePath + "rain.png";
            }
            else if (condition.Contains("Snow") || condition.Contains("snow"))
            {
                iconPath = basePath + "snow.png";
            }
            else if (condition.Contains("Thunderstorm"))
            {
                iconPath = basePath + "lightning.png";
            }
            else if (condition.Contains("Hurricane"))
            {
                iconPath = basePath + "hurricane.png";
            }

            // Log the selected icon path
            Console.WriteLine("Selected icon path: " + iconPath);
        }
        catch (Exception ex)
        {
            // Log the exception if any
            Console.WriteLine("Error selecting icon path: " + ex.Message);
        }

        return iconPath;
    }
}
