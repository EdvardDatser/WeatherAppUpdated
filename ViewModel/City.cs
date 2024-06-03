using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.ViewModel
{
    [Table("Cities")]
    public class City
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }

        [Unique, Column("cityName")]
        public string CityName { get; set; }

        public string temperature { get; set; }
        public string humidity { get; set; }
        public string condition { get; set; }
    }
}
