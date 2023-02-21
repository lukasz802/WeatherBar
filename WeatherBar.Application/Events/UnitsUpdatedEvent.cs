using WeatherBar.Application.Events.Templates;
using WeatherBar.Model.Enums;

namespace WeatherBar.Application.Events
{
    public class UnitsUpdatedEvent : EventBase<Units>
    {
        #region Constructors

        public UnitsUpdatedEvent(object source, Units content) : base(source, content)
        {
        }

        #endregion
    }
}
