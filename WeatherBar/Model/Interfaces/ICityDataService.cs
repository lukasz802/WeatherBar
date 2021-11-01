using System.Collections.Generic;

namespace WeatherBar.Model.Interfaces
{
    public interface ICityDataService
    {
        #region Public methods

        City GetCityById(string cityId);

        IEnumerable<City> GetCityListByName(string cityName);

        #endregion
    }
}
