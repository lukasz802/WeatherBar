using WebApi.Model.DataTransferObjects;

namespace WebApi.Interfaces
{
    public interface IWeatherApiClient
    {
        /// <summary>
        /// Gets current weather data by city name.
        /// </summary>
        HourlyForecastTransferObject GetCurrentWeatherDataByCityName(string cityName);

        /// <summary>
        /// Gets current weather data by city ID.
        /// </summary>
        HourlyForecastTransferObject GetCurrentWeatherDataByCityId(string cityId);

        /// <summary>
        /// Gets 4 days weather forecast data by city name.
        /// </summary>
        FourDaysForecastTransferObject GetFourDaysForecastDataByCityName(string cityName);

        /// <summary>
        /// Gets 4 days weather forecast data by city ID.
        /// </summary>
        FourDaysForecastTransferObject GetFourDaysForecastDataByCityId(string cityId);
    }
}
