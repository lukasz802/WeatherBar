using WebApi.Model.DataTransferObjects;

namespace WebApi.Interfaces
{
    public interface IWeatherApiClient
    {
        /// <summary>
        /// Gets current weather data.
        /// </summary>
        HourlyForecastTransferObject GetCurrentWeatherData(string cityData);

        /// <summary>
        /// Gets 4 days weather forecast data.
        /// </summary>
        FourDaysForecastTransferObject GetFourDaysForecastData(string cityData);
    }
}
