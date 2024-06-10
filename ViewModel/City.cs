using SQLite;
using System.ComponentModel;

namespace WeatherApp.ViewModel
{
    [Table("Cities")]
    public class City
    {
        public int Id { get; set; }
        public string CityName { get; set; }
        public string temperature { get; set; }
        public string humidity { get; set; }
        public string condition { get; set; }
        public string WeatherIconPath { get; set; } // Add this property
    }

}
