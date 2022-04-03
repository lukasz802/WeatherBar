using WeatherBar.Model.Interfaces;
using WebApi.Model.Enums;

namespace WeatherBar.Model.Services.Interfaces
{
    public interface IWeatherDataService
    {
        #region Public methods

        IHourlyData GetHourlyData(CallType callType, string cityData);

        IHourlyData GetEmptyHourlyData();

        IFourDaysData GetFourDaysData(CallType callType, string cityData);

        #endregion
    }
}
