using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WeatherBar.Model.Repositories;
using WeatherBar.Model.Repositories.Interfaces;
using WeatherBar.Model.Services.Interfaces;

namespace WeatherBar.Model.Services
{
    public class CityDataService : ICityDataService
    {
        #region Fields

        private readonly string databaseConnection = $"Data Source = {Path.Combine(Directory.GetCurrentDirectory(), "CityList.db")}";

        private readonly ICityRepository cityRepository;

        #endregion

        #region Constructors

        public CityDataService()
        {
            cityRepository = new CityRepository(databaseConnection);
        }

        #endregion

        #region Public methods

        public City GetCityById(string cityId)
        {
            return string.IsNullOrEmpty(cityId) ? null : cityRepository.GetAllWithId(cityId);
        }

        public IEnumerable<City> GetCityListByName(string cityName)
        {
            if (string.IsNullOrEmpty(cityName))
            {
                return Enumerable.Empty<City>();
            }

            var coordinatesList = new List<KeyValuePair<decimal, decimal>>();
            var result = new List<City>();

            foreach (City city in cityRepository.GetAllWithName(cityName))
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
