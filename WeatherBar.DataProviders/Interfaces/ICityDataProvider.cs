using System.Collections.Generic;
using WeatherBar.Model;

namespace WeatherBar.DataProviders.Interfaces
{
    public interface ICityDataProvider
    {
        #region Public methods

        City GetCityById(string cityId);

        IEnumerable<City> GetCityListByName(string cityName);

        #endregion
    }
}
