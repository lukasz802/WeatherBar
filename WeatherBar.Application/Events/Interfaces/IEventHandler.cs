namespace WeatherBar.Application.Events.Interfaces
{
    public interface IEventHandler<TContent> where TContent : IEvent
    {
        void Handle(TContent @event);
    }
}
