using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WeatherBar.WebApi.Models.Interfaces;

namespace WeatherBar.Utils
{
    public static class SharedFunctions
    {
        #region Public methods

        public static IEnumerable<IHourlyData> GetHourlyForecastForSpecificDate(IEnumerable<IHourlyData> hourlyData, string date)
        {
            return hourlyData.Where(x => x.Date.Contains(date.Trim().First() == '0' ? date.Trim().Remove(0,1) : date.Trim())).ToList();
        }

        public static Tuple<List<IHourlyData>, List<IHourlyData>> GetHourlyForecast(IEnumerable<IHourlyData> hourlyData)
        {
            return new Tuple<List<IHourlyData>, List<IHourlyData>>(hourlyData.Take(5).ToList(), hourlyData.ToList().GetRange(5, 5)); ;
        }

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

        public static BitmapImage LoadImage(Stream imageStream)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = imageStream;
            image.EndInit();
            image.Freeze();

            return image;
        }

        #endregion
    }
}
