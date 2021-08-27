using System;
using WebApi.Models.Enums;
using WebApi.Models.Interfaces;

namespace WebApi
{
    public interface IWeatherApi : IDisposable
    {
        /// <summary>
        /// Gets current weather data.
        /// </summary>
        IHourlyData GetCurrentWeatherData(string cityName);

        /// <summary>
        /// Gets 4 days weather forecast data.
        /// </summary>
        IFourDaysData GetFourDaysForecastData(string cityName);

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
