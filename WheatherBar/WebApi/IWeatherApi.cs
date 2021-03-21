using System;
using WeatherBar.WebApi.Models;
using WeatherBar.WebApi.Models.Enums;

namespace WeatherBar.WebApi
{
    public interface IWeatherApi : IDisposable
    {
        /// <summary>
        /// Get current weather data.
        /// </summary>
        CurrentWeatherData GetCurrentWeatherData(string cityName);

        /// <summary>
        /// Get 5 day weather forecast data.
        /// </summary>
        WeatherForecastData GetWeatherForecastData(string cityName);

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
