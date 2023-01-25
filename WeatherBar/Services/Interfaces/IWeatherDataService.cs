using WeatherBar.Model.Interfaces;

namespace WeatherBar.Services.Interfaces
{
    public interface IWeatherDataService
    {
        #region Public methods

        IHourlyData GetHourlyData(string cityData);

        IFourDaysData GetFourDaysData(string cityData);

        #endregion
    }
}
