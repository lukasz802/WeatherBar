using WeatherBar.Application.Events.Templates;

namespace WeatherBar.Application.Events
{
    public class ForecastPanelVisibilityChangedEvent : EventBase<bool>
    {
        #region Constructors

        public ForecastPanelVisibilityChangedEvent(object source, bool content) : base(source, content)
        {
        }

        #endregion
    }
}
