using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherBar.Model;

namespace WeatherBar.Persistence.Interfaces
{
    public interface ICityRepository
    {
        #region Public methods

        IEnumerable<City> GetAll();

        Task<IEnumerable<City>> GetAllAsync();

        City GetWithId(string cityId);

        Task<City> GetWithIdAsync(string cityId);

        IEnumerable<City> GetAllWithName(string cityName);

        Task<IEnumerable<City>> GetAllWithNameAsync(string cityName);

        #endregion
    }
}
