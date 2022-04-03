using System.Collections.Generic;

namespace WeatherBar.Model.Services.Interfaces
{
    public interface ICityDataService
    {
        #region Public methods

        City GetCityById(string cityId);

        IEnumerable<City> GetCityListByName(string cityName);

        #endregion
    }
}
