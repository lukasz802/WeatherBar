using WeatherBar.Application.Events.Templates;
using WeatherBar.Model;

namespace WeatherBar.Application.Events
{
    public class StartingLocationUpdatedEvent : EventBase<City>
    {
        #region Constructors

        public StartingLocationUpdatedEvent(object source, City content) : base(source, content)
        {
        }

        #endregion
    }
}
