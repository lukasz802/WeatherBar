using WeatherBar.Application.Events.Templates;
using WeatherBar.Model.Enums;

namespace WeatherBar.Application.Events
{
    public class AppStatusChangedEvent : EventBase<AppStatus>
    {
        #region Constructors

        public AppStatusChangedEvent(object source, AppStatus content) : base(source, content)
        {
        }

        #endregion
    }
}
