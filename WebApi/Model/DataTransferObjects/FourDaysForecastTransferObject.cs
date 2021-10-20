using System.Collections.Generic;
using WebApi.Model.Interfaces;

namespace WebApi.Model.DataTransferObjects
{
    public class FourDaysForecastTransferObject : IWeatherData
    {
        #region Properties

        public IEnumerable<HourlyForecastTransferObject> HourlyData { get; set; }

        public IEnumerable<DailyForecastTransferObject> DailyData { get; set; }

        #endregion
    }
}
