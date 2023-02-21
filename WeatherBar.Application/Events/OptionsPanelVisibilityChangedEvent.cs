using WeatherBar.Application.Events.Templates;

namespace WeatherBar.Application.Events
{
    public class OptionsPanelVisibilityChangedEvent : EventBase<bool>
    {
        #region Constructors

        public OptionsPanelVisibilityChangedEvent(object source, bool content) : base(source, content)
        {
        }

        #endregion
    }
}
