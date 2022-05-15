using System.Collections.Generic;
using WebApi.Model.Interfaces;

namespace WebApi.Model.DataTransferObjects
{
    public class FourDaysForecastTransferObject : IWeatherData
    {
        #region Properties

        public List<HourlyForecastTransferObject> HourlyData { get; set; }

        public List<DailyForecastTransferObject> DailyData { get; set; }

        #endregion
    }
}
