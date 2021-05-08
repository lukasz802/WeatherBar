using WeatherBar.WebApi.Models.Interfaces;
using WeatherBar.WebApi.Models.DataTransferObjects;

namespace WeatherBar.WebApi.Models.Factories
{
    public static class WeatherDataFactory
    {
        #region Public methods

        public static IHourlyData GetHourlyDataTransferObject()
        {
            return new HourlyForecast();
        }

        public static IDailyData GetDailyDataTransferObject()
        {
            return new DailyForecast();
        }

        public static IFourDaysData GetFourDaysDataTransferObject()
        {
            return new FourDaysForecast();
        }

        #endregion
    }
}
