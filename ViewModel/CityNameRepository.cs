using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.ViewModel
{
    public class CityNameRepository
    {
        SQLiteConnection database;
        public CityNameRepository(string databasePath)
        {
            database = new SQLiteConnection(databasePath);
            database.CreateTable<City>();
        }

        public IEnumerable<City> GetItems()
        {
            return database.Table<City>().ToList();
        }

        public City GetItem(int id)
        {
            return database.Get<City>(id);
        }

        public City SelectCityByName(string cityName)
        {
            return database.Find<City>(city => city.CityName == cityName);

        }

        public int DeleteItem(int id)
        {
            return database.Delete<City>(id);
        }

        public int SaveItem(City item)
        {

            try
            {
                if (item.Id != 0)
                {
                    database.Update(item);
                    return item.Id;
                }
                else
                {
                    return database.Insert(item);
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
