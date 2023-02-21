using WeatherBar.Application.Events.Templates;

namespace WeatherBar.Application.Events
{
    public class StartingLocationQueryResultUpdated : EventBase
    {
        #region Constructors

        public StartingLocationQueryResultUpdated(object source) : base(source)
        {
        }

        #endregion
    }
}
