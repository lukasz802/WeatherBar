using WeatherBar.Application.Events.Templates;
using WeatherBar.Model.Enums;

namespace WeatherBar.Application.Events
{
    public class LanguageUpdatedEvent : EventBase<Language>
    {
        #region Constructors

        public LanguageUpdatedEvent(object source, Language content) : base(source, content)
        {
        }

        #endregion
    }
}
