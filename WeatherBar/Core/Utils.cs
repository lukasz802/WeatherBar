using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WeatherBar.Models;
using WeatherBar.Models.Repositories;
using WebApi.Models.Interfaces;

namespace WeatherBar.Core
{
    public static class Utils
    {
        #region Fields

        private static readonly CityRepository cityRepository = new CityRepository();

        #endregion

        #region Public methods

        public static IEnumerable<IHourlyData> GetHourlyForecastForSpecificDate(IEnumerable<IHourlyData> hourlyData, string date)
        {
            return hourlyData.Where(x => x.Date.Contains(date.Trim().First() == '0' ? date.Trim().Remove(0,1) : date.Trim())).ToList();
        }

        public static Tuple<List<IHourlyData>, List<IHourlyData>> GetHourlyForecast(IEnumerable<IHourlyData> hourlyData)
        {
            return new Tuple<List<IHourlyData>, List<IHourlyData>>(hourlyData.Take(5).ToList(), hourlyData.ToList().GetRange(5, 5)); ;
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

        public static IEnumerable<City> GetCityList(string cityName)
        {
            if (string.IsNullOrEmpty(cityName))
            {
                return Enumerable.Empty<City>();
            }

            var coordinatesList = new List<KeyValuePair<decimal, decimal>>();
            var result = new List<City>();

            foreach (var city in cityRepository.GetAllWithName(cityName))
            {
                if (!coordinatesList.Any(x => Math.Floor(x.Key * 10) == Math.Floor(city.Latitude * 10) && Math.Floor(x.Value * 10) == Math.Floor(city.Longtitude * 10)))
                {
                    coordinatesList.Add(new KeyValuePair<decimal, decimal>(city.Latitude, city.Longtitude));
                    result.Add(city);
                }
            }

            return result;
        }

        #endregion
    }
}
