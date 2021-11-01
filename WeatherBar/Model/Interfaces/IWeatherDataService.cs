using WebApi.Model.Enums;

namespace WeatherBar.Model.Interfaces
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
