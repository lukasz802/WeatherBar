using System.Collections.Generic;

namespace WebApi.Model.Interfaces
{
    public interface IFourDaysData : IWeatherData
    {
        IEnumerable<IHourlyData> HourlyData { get; }

        IEnumerable<IDailyData> DailyData { get; }
    }
}
