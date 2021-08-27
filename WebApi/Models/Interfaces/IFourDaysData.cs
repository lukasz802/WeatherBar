using System.Collections.Generic;

namespace WebApi.Models.Interfaces
{
    public interface IFourDaysData : IWeatherData
    {
        IEnumerable<IHourlyData> HourlyData { get; set; }

        IEnumerable<IDailyData> DailyData { get; set; }
    }
}
