using WeatherBar.Application.Events.Templates;
using WeatherBar.Model;

namespace WeatherBar.Application.Events
{
    public class QueryResultUpdatedEvent : EventBase<QueryExecution>
    {
        #region Constructors

        public QueryResultUpdatedEvent(object source, QueryExecution content) : base(source, content)
        {
        }

        #endregion
    }
}
