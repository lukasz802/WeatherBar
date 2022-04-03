
namespace WebApi.Interfaces
{
    public interface IUpdateConfiguration
    {
        /// <summary>
        /// Update WeatherAPI configuration.
        /// </summary>
        void UpdateConfiguration(string cityId, int interval);
    }
}
