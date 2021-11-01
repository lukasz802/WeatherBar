using System.Collections.Generic;
using System.ComponentModel;

namespace WeatherBar.Model.Interfaces
{
    public interface IFourDaysData : INotifyPropertyChanged, IMultiLanguage, IUnits
    {
        IEnumerable<IHourlyData> HourlyData { get; }

        IEnumerable<IDailyData> DailyData { get; }
    }
}
