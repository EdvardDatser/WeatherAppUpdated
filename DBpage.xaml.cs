using Newtonsoft.Json.Linq;
using WeatherApp.ViewModel;

namespace WeatherApp;

public partial class DBpage : ContentPage
{
    MainPage mainPage = new MainPage();
    public DBpage()
    {
        InitializeComponent();
    }
    private async void SaveCity(object sender, EventArgs e)
    {
        var city = (City)BindingContext;
        if (!String.IsNullOrEmpty(city.CityName))
        {
            await RetrieveWeatherData(city.CityName, city);
        }
        await Navigation.PopAsync();
    }


    private void DeleteCity(object sender, EventArgs e)
    {
        var city = (City)BindingContext;
        App.Database.DeleteItem(city.Id);
        this.Navigation.PopAsync();
    }
    private void Cancel(object sender, EventArgs e)
    {
        this.Navigation.PopAsync();
    }



    private async void SelectCity(object sender, EventArgs e)
    {
        var city = (City)BindingContext;
        mainPage.anotherlocation = city.CityName;

        City selectedCity = App.Database.SelectCityByName(mainPage.anotherlocation);

        if (selectedCity != null)
        {

            mainPage.SelectFavoriteCity = true;
            await Navigation.PushAsync(mainPage);

        }
        else
        {
            // Обработка ситуации, когда город не найден
            await DisplayAlert("Error", "City not found", "OK");
        }
    }
    // добавляется температура при добавлении города
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

                // После получения температуры присваиваем ее объекту city
                city.temperature = temperature;
                city.humidity = humidity;
                city.condition = condition;

                // Сохраняем объект city в базе данных
                App.Database.SaveItem(city);
            }
            else
            {
                await DisplayAlert("Error", "City not found. Check the spelling of the city", "OK");
            }
        }
    }
}