using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WeatherBar.Model.Enums;
using WeatherBar.Model.Interfaces;
using WeatherBar.Model.Templates;

namespace WeatherBar.Model
{
    public class HourlyForecast : MultiLanguageBase, IUnits
    {
        #region Fields

        private readonly Dictionary<Language, string> dateDict = new Dictionary<Language, string>();

        private readonly Dictionary<Language, string> weekDayDict = new Dictionary<Language, string>();

        private readonly Dictionary<Language, string> dayTimeDict = new Dictionary<Language, string>();

        private readonly Dictionary<Language, string> descriptionDict = new Dictionary<Language, string>();

        #endregion

        #region Properties

        public Units Units { get; set; }

        public string Description
        {
            get
            {
                return descriptionDict.Count != 0 ? descriptionDict[Language] : null;
            }
        }

        public string DescriptionId { get; private set; }

        public string CityName { get; private set; }

        public string CityId { get; private set; }

        public int AvgTemp { get; private set; }

        public int FeelTemp { get; private set; }

        public double SnowFall { get; private set; }

        public double RainFall { get; private set; }

        public DateTime UpdateTime { get; private set; }

        public string Date
        {
            get
            {
                return dateDict.Count != 0 ? dateDict[Language] : null;
            }
        }

        public string DayTime
        {
            get
            {
                return dayTimeDict.Count != 0 ? dayTimeDict[Language] : null;
            }
        }

        public string SunsetTime { get; private set; }

        public string SunriseTime { get; private set; }

        public string Country { get; private set; }

        public double Longtitude { get; private set; }

        public double Latitude { get; private set; }

        public int Pressure { get; private set; }

        public int Humidity { get; private set; }

        public int WindSpeed { get; private set; }

        public int WindAngle { get; private set; }

        public string Icon { get; private set; }

        public string WeekDay
        {
            get
            {
                return weekDayDict.Count != 0 ? weekDayDict[Language] : null;
            }
        }

        #endregion

        #region Constructors

        public HourlyForecast(string description, int avgTemp, int feelTemp, double snowFall, double rainFall, int pressure, int humidity, int windSpeed,
            int windAngle, string icon, string country = null, string cityName = null, string cityId = null, string sunsetTime = null, string sunriseTime = null,
            double? longitude = null, double? latitude = null, string descriptionId = null, DateTime? date = null, string dayTime = null)
        {
            PrepareDictionaries(Language.Polish, description, descriptionId, date, dayTime);
            PrepareDictionaries(Language.English, description, descriptionId, date, dayTime);

            Language = Language.Polish;
            Units = Units.Metric;
            DescriptionId = descriptionId;
            CityName = cityName;
            CityId = cityId;
            AvgTemp = avgTemp;
            FeelTemp = feelTemp;
            SnowFall = snowFall;
            RainFall = rainFall;
            WindAngle = windAngle;
            Icon = icon;
            SunsetTime = sunsetTime;
            SunriseTime = sunriseTime;
            Longtitude = longitude.GetValueOrDefault();
            Latitude = latitude.GetValueOrDefault();
            Pressure = pressure;
            Humidity = humidity;
            Country = country;
            WindSpeed = windSpeed;
            UpdateTime = DateTime.Now;
        }

        public HourlyForecast()
        {
            Language = Language.Polish;
            Units = Units.Metric;
            Icon = "01d";
            DescriptionId = "800";
            UpdateTime = DateTime.Now;
        }

        #endregion

        #region Public methods

        public static HourlyForecast Empty()
        {
            return new HourlyForecast();
        }

        public override void ChangeLanguage(Language language)
        {
            Language = language;
        }

        public void ChangeUnits(Units units)
        {
            SetUnits(units, Units);

            Units = units;
        }

        #endregion

        #region Private methods

        private void PrepareDictionaries(Language language, string description, string descriptionId, DateTime? date, string dayTime)
        {
            var cultureName = new CultureInfo(language == Language.English ? "en-US" : "pl-PL");

            if (dayTime == null)
            {
                dayTimeDict.Add(language, language == Language.Polish ? "Teraz" : "Now");
            }
            else
            {
                dayTimeDict.Add(language, dayTime);
            }

            if (language == Language.Polish)
            {
                descriptionDict.Add(language, description.FirstOrDefault().ToString().ToUpper() + description.Substring(1));
            }
            else
            {
                descriptionDict.Add(language, GetDescriptionFromId(descriptionId));
            }

            if (date != null)
            {
                weekDayDict.Add(language, cultureName.DateTimeFormat.GetDayName(date.Value.DayOfWeek).FirstOrDefault().ToString().ToUpper() +
                        cultureName.DateTimeFormat.GetDayName(date.Value.DayOfWeek).Substring(1));
                dateDict.Add(language, date.Value.ToString("dd MMMM", cultureName).First() == '0' ? date.Value.ToString("dd MMMM", cultureName).Remove(0, 1) : date.Value.ToString("dd MMMM", cultureName));
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

        private void CalculateFromMetricToImperial()
        {
            AvgTemp = Convert.ToInt32(Math.Round((AvgTemp * 9 / 5) + 32D, MidpointRounding.AwayFromZero));
            FeelTemp = Convert.ToInt32(Math.Round((FeelTemp * 9 / 5) + 32D, MidpointRounding.AwayFromZero));
            WindSpeed = Convert.ToInt32(Math.Round(0.621371192 * WindSpeed, MidpointRounding.AwayFromZero));
        }

        private void CalculateFromImperialToMetric()
        {
            AvgTemp = Convert.ToInt32(Math.Round((AvgTemp - 32) * 5 / 9D, MidpointRounding.AwayFromZero));
            FeelTemp = Convert.ToInt32(Math.Round((FeelTemp - 32) * 5 / 9D, MidpointRounding.AwayFromZero));
            WindSpeed = Convert.ToInt32(Math.Round(1.609344 * WindSpeed, MidpointRounding.AwayFromZero));
        }

        private void CalculateFromMetricToStandard()
        {
            AvgTemp += 273;
            FeelTemp += 273;
        }

        private void CalculateFromStandardToMetric()
        {
            AvgTemp -= 273;
            FeelTemp -= 273;
        }

        #endregion
    }
}
