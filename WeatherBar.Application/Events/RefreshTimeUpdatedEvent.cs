using WeatherBar.Application.Events.Templates;
using WeatherBar.Model.Enums;

namespace WeatherBar.Application.Events
{
    public class RefreshTimeUpdatedEvent : EventBase<RefreshTime>
    {
        #region Constructors

        public RefreshTimeUpdatedEvent(object source, RefreshTime content) : base(source, content)
        {
        }

        #endregion
    }
}
