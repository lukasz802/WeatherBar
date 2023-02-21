namespace WeatherBar.Application.Events.Interfaces
{
    public interface IEvent<TContent> : IEvent
    {
        TContent Content { get; }
    }

    public interface IEvent
    {
        object Source { get; }
    }
}
