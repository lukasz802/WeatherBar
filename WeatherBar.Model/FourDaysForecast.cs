using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WeatherBar.Model.Enums;
using WeatherBar.Model.Interfaces;
using WeatherBar.Model.Templates;

namespace WeatherBar.Model
{
    public class FourDaysForecast : MultiLanguageBase, IUnits
    {
        #region Fields

        private readonly List<HourlyForecast> hourlyData;

        private readonly List<DailyForecast> dailyData;

        #endregion

        #region Properties

        public List<HourlyForecast> HourlyData => new List<HourlyForecast>(hourlyData);

        public List<DailyForecast> DailyData => new List<DailyForecast>(dailyData);

        public Units Units { get; private set; }

        #endregion

        #region Constructors

        public FourDaysForecast(List<HourlyForecast> hourlyData, List<DailyForecast> dailyData)
        {
            this.hourlyData = hourlyData;
            this.dailyData = dailyData;
        }

        private FourDaysForecast()
        {
            hourlyData = new List<HourlyForecast>();
            dailyData = new List<DailyForecast>();
        }

        #endregion

        #region Public methods

        public static FourDaysForecast Empty()
        {
            return new FourDaysForecast();
        }

        public override void ChangeLanguage(Language language)
        {
            Language = language;

            dailyData.ForEach(x => x.ChangeLanguage(Language));
            hourlyData.ForEach(x => x.ChangeLanguage(Language));
        }

        public void ChangeUnits(Units units)
        {
            Units = units;

            dailyData.ForEach(x => x.ChangeUnits(Units));
            hourlyData.ForEach(x => x.ChangeUnits(Units));
        }

        public IEnumerable<HourlyForecast> GetHourlyForecastForDate(DateTime date)
        {
            var cultureName = new CultureInfo(Language == Language.English ? "en-US" : "pl-PL");
            var tempDate = date.ToString("dd MMMM", cultureName).Trim();

            return HourlyData.Where(x => x.Date.Contains(tempDate.First() == '0' ? tempDate.Remove(0, 1) : tempDate));
        }

        #endregion
    }
}
