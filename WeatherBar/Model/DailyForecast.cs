using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using WeatherBar.Model.Enums;
using WeatherBar.Model.Interfaces;
using WeatherBar.Utils;

namespace WeatherBar.Model
{
    public class DailyForecast : IDailyData
    {
        #region Fields

        private readonly Dictionary<Language, string> dateDict = new Dictionary<Language, string>();

        private readonly Dictionary<Language, string> descriptionDict = new Dictionary<Language, string>();

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Propeties

        public int MaxTemp { get; private set; }

        public int MinTemp { get; private set; }

        public string Date
        {
            get
            {
                return dateDict.Count != 0 ? dateDict[Language] : null;
            }
        }

        public string Icon { get; private set; }

        public string Description 
        { 
            get
            {
                return descriptionDict.Count != 0 ? descriptionDict[Language] : null;
            }
        }

        public string DescriptionId { get; private set; }

        public Language Language { get; private set; }

        public Units Units { get; private set; }

        #endregion

        #region Construcors

        public DailyForecast(double maxTemp, double minTemp, string icon, string description, string descriptionId, DayOfWeek weekDay, string date)
        {
            PrepareDictionaries(Language.Polish, description, descriptionId, weekDay, date);
            PrepareDictionaries(Language.English, description, descriptionId, weekDay, date);

            Language = Language.Polish;
            Units = Units.Metric;
            DescriptionId = descriptionId;
            MaxTemp = Convert.ToInt32(maxTemp);
            MinTemp = Convert.ToInt32(minTemp);
            Icon = icon;
        }

        #endregion

        #region Public methods

        public void ChangeLanguage(Language language)
        {
            Language = language;

            OnPropertyChanged("Description");
            OnPropertyChanged("Date");
        }

        public void ChangeUnits(Units units)
        {
            SetUnits(units, Units);

            Units = units;
            OnPropertyChanged("MinTemp");
            OnPropertyChanged("MaxTemp");
        }

        #endregion

        #region Private methods

        private void PrepareDictionaries(Language language, string description, string descriptionId, DayOfWeek weekDay, string date)
        {
            var cultureName = new CultureInfo(language == Language.English ? "en-US" : "pl-PL");

            Match local = Regex.Match(date, @"(?<Month>(\d{1,2}))-(?<Day>(\d{1,2}))", RegexOptions.RightToLeft);

            dateDict.Add(language, cultureName.DateTimeFormat.GetAbbreviatedDayName(weekDay) + ", " +
                           int.Parse(local.Groups["Day"].Value).ToString() + " " + cultureName.DateTimeFormat.GetAbbreviatedMonthName(int.Parse(local.Groups["Month"].Value)));

            if (language == Language.Polish)
            {
                descriptionDict.Add(language, cultureName.DateTimeFormat.GetDayName(weekDay).First().ToString().ToUpper() +
                           cultureName.DateTimeFormat.GetDayName(weekDay).Substring(1) + ", " + description);
            }
            else
            {
                descriptionDict.Add(language, cultureName.DateTimeFormat.GetDayName(weekDay).First().ToString().ToUpper() +
                           cultureName.DateTimeFormat.GetDayName(weekDay).Substring(1) + ", " + ApplicationUtils.GetDescriptionFromId(descriptionId));
            }
        }

        private void SetUnits(Units toUnits, Units fromUnits)
        {
            switch (toUnits)
            {
                case Units.Metric:
                    if (fromUnits == Units.Imperial)
                    {
                        CalculateFromImperialToMetric();
                    }
                    else if (fromUnits == Units.Standard)
                    {
                        CalculateFromStandardToMetric();
                    }
                    break;
                case Units.Imperial:
                    if (fromUnits == Units.Metric)
                    {
                        CalculateFromMetricToImperial();
                    }
                    else if (fromUnits == Units.Standard)
                    {
                        CalculateFromStandardToMetric();
                        CalculateFromMetricToImperial();
                    }
                    break;
                case Units.Standard:
                    if (fromUnits == Units.Metric)
                    {
                        CalculateFromMetricToStandard();
                    }
                    else if (fromUnits == Units.Imperial)
                    {
                        CalculateFromImperialToMetric();
                        CalculateFromMetricToStandard();
                    }
                    break;
            }
        }

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CalculateFromMetricToImperial()
        {
            MaxTemp = Convert.ToInt32(Math.Round((MaxTemp * 9 / 5) + 32D, MidpointRounding.AwayFromZero));
            MinTemp = Convert.ToInt32(Math.Round((MinTemp * 9 / 5) + 32D, MidpointRounding.AwayFromZero));
        }

        private void CalculateFromImperialToMetric()
        {
            MaxTemp = Convert.ToInt32(Math.Round((MaxTemp - 32) * 5 / 9D, MidpointRounding.AwayFromZero));
            MinTemp = Convert.ToInt32(Math.Round((MinTemp - 32) * 5 / 9D, MidpointRounding.AwayFromZero));
        }

        private void CalculateFromMetricToStandard()
        {
            MaxTemp += 273;
            MinTemp += 273;
        }

        private void CalculateFromStandardToMetric()
        {
            MaxTemp -= 273;
            MinTemp -= 273;
        }

        #endregion
    }
}
