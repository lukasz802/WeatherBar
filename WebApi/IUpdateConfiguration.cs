using WebApi.Model.Enums;

namespace WebApi
{
    public interface IUpdateConfiguration
    {
        /// <summary>
        /// Update WeatherAPI configuration.
        /// </summary>
        void UpdateConfiguration(string cityId, Units units, int interval);
    }
}
