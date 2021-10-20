using System;
using System.Collections.Generic;
using System.Linq;
using WeatherBar.Model.Interfaces;
using WeatherBar.Model.Repositories;

namespace WeatherBar.Model.Services
{
    public class CityDataService : ICityDataService
    {
        #region Fields

        private readonly ICityRepository cityRepository;

        #endregion

        #region Constructors

        public CityDataService()
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

            return cityRepository.GetAllWithId(cityId);
        }

        public IEnumerable<City> GetCityListByName(string cityName)
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
