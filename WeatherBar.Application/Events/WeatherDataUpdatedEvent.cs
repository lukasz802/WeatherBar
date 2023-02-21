using WeatherBar.Application.Events.Templates;

namespace WeatherBar.Application.Events
{
    public class WeatherDataUpdatedEvent : EventBase<string>
    {
        #region Constructor

        public WeatherDataUpdatedEvent(object source, string content) : base(source, content)
        {
        }

        #endregion
    }
}
