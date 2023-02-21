using WeatherBar.Application.Events.Interfaces;

namespace WeatherBar.Application.Events.Templates
{
    public abstract class EventBase<T> : IEvent<T>
    {
        #region Properties

        public object Source { get; }

        public T Content { get; }

        #endregion

        #region Constructors

        public EventBase(object source, T content)
        {
            Source = source;
            Content = content;
        }

        #endregion
    }

    public abstract class EventBase : IEvent
    {
        #region Properties

        public object Source { get; }

        #endregion

        #region Constructors

        public EventBase(object source)
        {
            Source = source;
        }

        #endregion
    }
}
