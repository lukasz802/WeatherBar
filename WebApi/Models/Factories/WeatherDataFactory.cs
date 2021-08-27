using WebApi.Models.Interfaces;
using WebApi.Models.DataTransferObjects;

namespace WebApi.Models.Factories
{
    public static class WeatherDataFactory
    {
        #region Public methods

        public static IHourlyData GetHourlyDataTransferObject()
        {
            return new HourlyForecastTransferObject();
        }

        public static IDailyData GetDailyDataTransferObject()
        {
            return new DailyForecastTransferObject();
        }

        public static IFourDaysData GetFourDaysDataTransferObject()
        {
            return new FourDaysForecastTransferObject();
        }

        #endregion
    }
}
