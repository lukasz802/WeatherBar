using System.Collections.Generic;

namespace WeatherBar.Model.Interfaces
{
    public interface IFourDaysData : IMultiLanguage, IUnits, IClonable<IFourDaysData>
    {
        List<IHourlyData> HourlyData { get; }

        List<IDailyData> DailyData { get; }
    }
}
