using System;
using System.Windows.Threading;
using WeatherBar.Model.Enums;

namespace WeatherBar.Core
{
    public class EventDispatcher
    {
        #region Fields

        private readonly DispatcherTimer timer = new DispatcherTimer();

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

        public void UpdateInterval(RefreshTime interval)
        {
            timer.Stop();
            timer.Interval = TimeSpan.FromMinutes((int)interval);
            timer.Start();
        }

        #endregion
    }
}
