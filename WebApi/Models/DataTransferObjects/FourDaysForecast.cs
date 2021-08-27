using System.Collections.Generic;
using WeatherBar.WebApi.Models.Interfaces;

namespace WeatherBar.WebApi.Models.DataTransferObjects
{
    public class FourDaysForecast : IFourDaysData
    {
        #region Properties

        public IEnumerable<IHourlyData> HourlyData { get; set; }

        public IEnumerable<IDailyData> DailyData { get; set; }

        #endregion

        #region Constructors

        public FourDaysForecast(IEnumerable<IHourlyData> hourlyData, IEnumerable<IDailyData> dailyData)
        {
            HourlyData = hourlyData;
            DailyData = dailyData;
        }

        public FourDaysForecast()
        {
        }

        #endregion
    }
}
