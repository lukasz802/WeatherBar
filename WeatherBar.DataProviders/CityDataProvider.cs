using System;
using System.Collections.Generic;
using System.Linq;
using WeatherBar.DataProviders.Interfaces;
using WeatherBar.Model;
using WeatherBar.Persistence;
using WeatherBar.Persistence.Interfaces;

namespace WeatherBar.DataProviders
{
    public class CityDataProvider : ICityDataProvider
    {
        #region Fields

        private readonly ICityRepository cityRepository;

        #endregion

        #region Constructors

        public CityDataProvider()
        {
            cityRepository = new CityRepository();
        }

        #endregion

        #region Public methods

        public City GetCityById(string cityId)
        {
            if (string.IsNullOrEmpty(cityId))
            {
                return null;
            }

            return cityRepository.GetWithId(cityId);
        }

        public IEnumerable<City> GetCityListByName(string cityName)
        {
            if (string.IsNullOrEmpty(cityName))
            {
                return Enumerable.Empty<City>();
            }

            var coordinatesList = new List<KeyValuePair<double, double>>();
            var result = new List<City>();

            foreach (City city in cityRepository.GetAllWithName(cityName))
            {
                if (!coordinatesList.Any(x => Math.Floor(x.Key * 10) == Math.Floor(city.Latitude * 10) && Math.Floor(x.Value * 10) == Math.Floor(city.Longtitude * 10)))
                {
                    coordinatesList.Add(new KeyValuePair<double, double>(city.Latitude, city.Longtitude));
                    result.Add(city);
                }
            }

            return result;
        }

        #endregion
    }
}
