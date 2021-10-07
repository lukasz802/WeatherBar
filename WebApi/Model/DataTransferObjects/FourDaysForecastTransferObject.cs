using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WebApi.Model.Enums;
using WebApi.Model.Interfaces;

namespace WebApi.Model.DataTransferObjects
{
    public class FourDaysForecastTransferObject : IFourDaysData
    {
        #region Properties

        public IEnumerable<IHourlyData> HourlyData { get; }

        public IEnumerable<IDailyData> DailyData { get; }

        public Language Language { get; set; }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public FourDaysForecastTransferObject(IEnumerable<IHourlyData> hourlyData, IEnumerable<IDailyData> dailyData)
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

        #endregion

        #region Private methods

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
