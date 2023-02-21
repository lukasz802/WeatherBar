using WeatherBar.Model;

namespace WeatherBar.DataProviders.Interfaces
{
    public interface IWeatherDataProvider
    {
        HourlyForecast GetCurrentForecast(string cityData);

        FourDaysForecast GetFourDaysForecast(string cityData);
    }
}
