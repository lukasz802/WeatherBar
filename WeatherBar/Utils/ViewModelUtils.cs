using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using WeatherBar.Model;
using WeatherBar.Model.Repositories;
using WebApi.Model.Enums;
using WebApi.Model.Interfaces;

namespace WeatherBar.Utils
{
    public static class ViewModelUtils
    {
        #region Fields

        private static readonly CityRepository cityRepository = new CityRepository();

        #endregion

        #region Public methods

        public static IEnumerable<IHourlyData> GetHourlyForecastForSpecificDate(IEnumerable<IHourlyData> hourlyData, Language language, DateTime date)
        {
            var cultureName = new CultureInfo(language == Language.English ? "en-US" : "pl-PL");
            var tempDate = date.ToString("dd MMMM", cultureName).Trim();

            return hourlyData.Where(x => x.Date.Contains(tempDate.First() == '0' ? tempDate.Remove(0, 1) : tempDate)).ToList();
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
