using System;
using WeatherBar.Application.Events.Templates;

namespace WeatherBar.Application.Events
{
    public class ShowDailyForecastEvent : EventBase<DateTime>
    {
        #region Constructors

        public ShowDailyForecastEvent(object source, DateTime content) : base(source, content)
        {
        }

        #endregion
    }
}
