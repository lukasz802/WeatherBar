using System.Collections.Generic;
using WebApi.Models.Interfaces;

namespace WebApi.Models.DataTransferObjects
{
    public class FourDaysForecastTransferObject : IFourDaysData
    {
        #region Properties

        public IEnumerable<IHourlyData> HourlyData { get; set; }

        public IEnumerable<IDailyData> DailyData { get; set; }

        #endregion

        #region Constructors

        public FourDaysForecastTransferObject(IEnumerable<IHourlyData> hourlyData, IEnumerable<IDailyData> dailyData)
        {
            HourlyData = hourlyData;
            DailyData = dailyData;
        }

        public FourDaysForecastTransferObject()
        {
        }

        #endregion
    }
}
