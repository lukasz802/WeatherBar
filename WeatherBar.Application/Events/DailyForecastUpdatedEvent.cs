using System.Collections.Generic;
using WeatherBar.Application.Events.Templates;
using WeatherBar.Model;

namespace WeatherBar.Application.Events
{
    public class DailyForecastUpdatedEvent : EventBase<List<HourlyForecast>>
    {
        #region Constructors

        public DailyForecastUpdatedEvent(object source, List<HourlyForecast> content) : base(source, content)
        {
        }

        #endregion
    }
}
