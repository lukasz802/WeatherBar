using System;
using System.Windows.Threading;
using WeatherBar.Model.Enums;

namespace WeatherBar.Application.Dispatchers
{
    public class EventDispatcher
    {
        #region Fields

        private static Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background, dispatcher);

        #endregion

        #region Constructors

        public EventDispatcher(Action action, int interval, bool autoReset = false)
        {
            timer.Interval = TimeSpan.FromMilliseconds(interval);
            timer.Tick += (s, a) =>
            {
                if (!autoReset)
                {
                    timer.Stop();
                }

                action.Invoke();
            };
        }

        public EventDispatcher(Action action, RefreshTime interval, bool autoReset = false) 
            : this(action, (int)interval * 60 * 1000, autoReset)
        {
        }

        #endregion

        #region Public methods

        public static void RaiseEventWithDelay(Action action, int delay = 0)
        {
            var timer = new DispatcherTimer(DispatcherPriority.Background, dispatcher)
            {
                Interval = TimeSpan.FromMilliseconds(delay)
            };

            timer.Tick += (s, a) =>
            {
                timer.Stop();
                action.Invoke();
            };

            timer.Start();
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void Restart()
        {
            timer.Stop();
            timer.Start();
        }

        public void UpdateInterval(RefreshTime interval)
        {
            timer.Interval = TimeSpan.FromMinutes((int)interval);
        }

        public void UpdateDispatcher(Dispatcher dispatcher)
        {
            EventDispatcher.dispatcher = dispatcher;

            timer = new DispatcherTimer(DispatcherPriority.Background, EventDispatcher.dispatcher);
        }

        #endregion
    }
}
