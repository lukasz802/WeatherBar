using WeatherBar.Application.Events.Templates;
using WeatherBar.Model;

namespace WeatherBar.Application.Events
{
    public class CurrentWeatherDataUpdatedEvent : EventBase<HourlyForecast>
    {
        #region Constructors

        public CurrentWeatherDataUpdatedEvent(object source, HourlyForecast content) : base(source, content)
        {
        }

        #endregion
    }
}
