using System;
using WeatherBar.WebApi.Models.Enums;
using WeatherBar.WebApi.Models.Interfaces;

namespace WeatherBar.WebApi
{
    public interface IWeatherApi : IDisposable
    {
        /// <summary>
        /// Get current weather data.
        /// </summary>
        IHourlyData GetCurrentWeatherData(string cityName);

        /// <summary>
        /// Get 4 days weather forecast data.
        /// </summary>
        IFourDaysData GetFourDaysForecastData(string cityName);

        /// <summary>
        /// Get unique API key.
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// Units of measurement.
        /// </summary>
        Units Units { get; set; }
    }
}
