using WeatherApp.ViewModel;

namespace WeatherApp;

public partial class App : Application
{
    public const string WeatherDB = "weatherDB";
    public static CityNameRepository database;
    public static CityNameRepository Database
    {
        get
        {
            if (database == null)
            {
                database = new CityNameRepository(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), WeatherDB));
            }
            return database;
        }
    }
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}
