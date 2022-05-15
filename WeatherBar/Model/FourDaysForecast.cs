using System.Collections.Generic;
using WeatherBar.Model.Enums;
using WeatherBar.Model.Interfaces;

namespace WeatherBar.Model
{
    public class FourDaysForecast : IFourDaysData
    {
        #region Properties

        public List<IHourlyData> HourlyData { get; private set; }

        public List<IDailyData> DailyData { get; private set; }

        public Language Language { get; set; }

        public Units Units { get; set; }

        #endregion

        #region Constructors

        public FourDaysForecast(List<IHourlyData> hourlyData, List<IDailyData> dailyData)
        {
            HourlyData = hourlyData;
            DailyData = dailyData;
        }

        #endregion

        #region Public methods

        public void ChangeLanguage(Language language)
        {
            Language = language;

            DailyData.ForEach(x => x.ChangeLanguage(Language));
            HourlyData.ForEach(x => x.ChangeLanguage(Language));
        }

        public void ChangeUnits(Units units)
        {
            Units = units;

            DailyData.ForEach(x => x.ChangeUnits(Units));
            HourlyData.ForEach(x => x.ChangeUnits(Units));
        }

        public IFourDaysData Clone()
        {
            var newHourlyData = new List<IHourlyData>();

            HourlyData.ForEach(item =>
            {
                newHourlyData.Add(item.Clone());
            });

            var newDailyData = new List<IDailyData>();

            DailyData.ForEach(item =>
            {
                newDailyData.Add(item.Clone());
            });

            return new FourDaysForecast(newHourlyData, newDailyData);
        }

        #endregion
    }
}
