using System;

namespace WebApi.Interfaces
{
    public interface IWeatherApi : IDisposable, IUpdateConfiguration, IWeatherApiClient
    {
        /// <summary>
        /// Gets unique API key.
        /// </summary>
        string ApiKey { get; }

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
