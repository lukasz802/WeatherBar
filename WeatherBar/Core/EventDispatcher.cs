using System;
using System.Windows.Threading;

namespace WeatherBar.Core
{
    public class EventDispatcher
    {
        #region Fields

        private readonly DispatcherTimer timer = new DispatcherTimer();

        #endregion

        #region Constructors

        public EventDispatcher(Action action, int interval)
        {
            timer.Interval = TimeSpan.FromMilliseconds(interval);
            timer.Tick += (s, a) =>
            {
                timer.Stop();
                action.Invoke();
            };
        }

        #endregion

        #region Public methods

        public static void RaiseEventWithDelay(Action action, int delay = 0)
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(delay)
            };

            timer.Start();
            timer.Tick += (s, a) =>
            {
                timer.Stop();
                action.Invoke();
            };
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

        #endregion
    }
}
