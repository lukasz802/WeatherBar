using System;
using WebApi.Models.Enums;
using WebApi.Models.Interfaces;

namespace WebApi
{
    public interface IWeatherApi : IDisposable
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

        /// <summary>
        /// Gets unique API key.
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// Gets or sets units of measurement.
        /// </summary>
        Units Units { get; set; }

        /// <summary>
        /// Gets or sets refresh interval in minutes.
        /// </summary>
        int Interval { get; set; }
    }
}
