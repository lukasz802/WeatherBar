using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WeatherBar.Model.Enums;
using WeatherBar.Model.Interfaces;

namespace WeatherBar.Model
{
    public class FourDaysForecast : IFourDaysData
    {
        #region Properties

        public IEnumerable<IHourlyData> HourlyData { get; private set; }

        public IEnumerable<IDailyData> DailyData { get; private set; }

        public Language Language { get; set; }

        public Units Units { get; set; }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public FourDaysForecast(IEnumerable<IHourlyData> hourlyData, IEnumerable<IDailyData> dailyData)
        {
            HourlyData = hourlyData;
            DailyData = dailyData;
        }

        #endregion

        #region Public methods

        public void ChangeLanguage(Language language)
        {
            Language = language;

            DailyData.ToList().ForEach(x => x.ChangeLanguage(Language));
            HourlyData.ToList().ForEach(x => x.ChangeLanguage(Language));

            OnPropertyChanged("DailyData");
            OnPropertyChanged("HourlyData");
        }

        public void ChangeUnits(Units units)
        {
            Units = units;

            DailyData.ToList().ForEach(x => x.ChangeUnits(Units));
            HourlyData.ToList().ForEach(x => x.ChangeUnits(Units));

            OnPropertyChanged("DailyData");
            OnPropertyChanged("HourlyData");
        }

        #endregion

        #region Private methods

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
