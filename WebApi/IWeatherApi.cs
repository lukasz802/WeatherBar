using System;
using WebApi.Model.Enums;

namespace WebApi
{
    public interface IWeatherApi : IDisposable, IUpdateConfiguration, IWeatherApiCommands
    {
        /// <summary>
        /// Gets unique API key.
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// Gets units of measurement.
        /// </summary>
        Units Units { get; }

        /// <summary>
        /// Gets refresh interval in minutes.
        /// </summary>
        int Interval { get; }

        /// <summary>
        /// Gets city ID.
        /// </summary>
        string CityId { get; }
    }
}
