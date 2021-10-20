using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeatherBar.Model.Interfaces
{
    public interface ICityRepository
    {
        #region Public methods

        IEnumerable<City> GetAll();

        Task<IEnumerable<City>> GetAllAsync();

        City GetAllWithId(string cityId);

        Task<City> GetAllWithIdAsync(string cityId);

        IEnumerable<City> GetAllWithName(string cityName);

        Task<IEnumerable<City>> GetAllWithNameAsync(string cityName);

        #endregion
    }
}
