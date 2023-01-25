using System.Collections.Generic;
using WeatherBar.Model;

namespace WeatherBar.Services.Interfaces
{
    public interface ICityDataService
    {
        #region Public methods

        City GetCityById(string cityId);

        IEnumerable<City> GetCityListByName(string cityName);

        #endregion
    }
}
