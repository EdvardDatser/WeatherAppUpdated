﻿using Newtonsoft.Json.Linq;
using WeatherApp.ViewModel;

namespace WeatherApp;

public partial class DBListPage : ContentPage
{
    public DBListPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        cityList.ItemsSource = App.Database.GetItems();
        base.OnAppearing();
        UpdateWeather();
    }

    private async void UpdateWeather()
    {
        //должно обновлять температуру при запуске этой страницы (еще не проверял)
        // Получаем список городов из базы данных
        var cities = App.Database.GetItems();

        // Перебираем каждый город и обновляем его температуру
        foreach (var city in cities)
        {
            await RetrieveWeatherData(city.CityName, city);
        }

        // Обновляем источник данных для списка городов
        cityList.ItemsSource = cities;
    }

    private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        City selectedCity = (City)e.SelectedItem;
        DBpage dBpage = new DBpage();
        dBpage.BindingContext = selectedCity;
        await Navigation.PushAsync(dBpage);
    }

    public async void AddFavorite(object sender, EventArgs e)
    {
        City city = new City();
        DBpage dBpage = new DBpage();
        dBpage.BindingContext = city;
        await Navigation.PushAsync(dBpage);
    }

    public async Task RetrieveWeatherData(string location, City city)
    {
        string url = $"https://api.weatherapi.com/v1/current.json?key=aa7758b35f384a5eb62102337241405&q={location}&aqi=no";

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(jsonResponse);
                string temperature = data["current"]["temp_c"].ToString();

                // После получения температуры присваиваем ее объекту city
                city.temperature = temperature;

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