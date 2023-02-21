using WeatherBar.Application.Events.Templates;

namespace WeatherBar.Application.Events
{
    public class WeatherDataRefreshedEvent : EventBase
    {
        #region Constructor

        public WeatherDataRefreshedEvent(object source) : base(source)
        {
        }

        #endregion
    }
}
