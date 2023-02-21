using WeatherBar.Application.Events.Templates;
using WeatherBar.Model;

namespace WeatherBar.Application.Events
{
    public class FourDaysForecastDataUpdatedEvent : EventBase<FourDaysForecast>
    {
        #region Constructors

        public FourDaysForecastDataUpdatedEvent(object source, FourDaysForecast content) : base(source, content)
        {
        }

        #endregion
    }
}
