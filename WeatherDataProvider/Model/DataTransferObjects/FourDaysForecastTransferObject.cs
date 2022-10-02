using System.Collections.Generic;
using WeatherDataProvider.Model.Interfaces;

namespace WeatherDataProvider.Model.DataTransferObjects
{
    public class FourDaysForecastTransferObject : IWeatherData
    {
        #region Properties

        public List<HourlyForecastTransferObject> HourlyData { get; set; }

        public List<DailyForecastTransferObject> DailyData { get; set; }

        #endregion
    }
}
