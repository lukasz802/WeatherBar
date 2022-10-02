using WeatherDataProvider.Model.DataTransferObjects;

namespace WeatherDataProvider.Interfaces
{
    public interface IWeatherDataProvider
    {
        /// <summary>
        /// Gets unique API key.
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// Gets current weather forecast data.
        /// </summary>
        HourlyForecastTransferObject GetCurrentForecast(string cityData);

        /// <summary>
        /// Gets 4 days weather forecast data.
        /// </summary>
        FourDaysForecastTransferObject GetFourDaysForecast(string cityData);
    }
}
