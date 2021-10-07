using WebApi.Model.Interfaces;

namespace WebApi
{
    public interface IWeatherApiCommands
    {
        /// <summary>
        /// Gets current weather data by city name.
        /// </summary>
        IHourlyData GetCurrentWeatherDataByCityName(string cityName);

        /// <summary>
        /// Gets current weather data by city ID.
        /// </summary>
        IHourlyData GetCurrentWeatherDataByCityId(string cityId);

        /// <summary>
        /// Gets 4 days weather forecast data by city name.
        /// </summary>
        IFourDaysData GetFourDaysForecastDataByCityName(string cityName);

        /// <summary>
        /// Gets 4 days weather forecast data by city ID.
        /// </summary>
        IFourDaysData GetFourDaysForecastDataByCityId(string cityId);
    }
}
