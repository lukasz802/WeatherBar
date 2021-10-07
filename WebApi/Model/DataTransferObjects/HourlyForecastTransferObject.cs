using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using WebApi.Model.Enums;
using WebApi.Model.Interfaces;

namespace WebApi.Model.DataTransferObjects
{
    public class HourlyForecastTransferObject : IHourlyData
    {
        #region Fields

        private readonly Dictionary<Language, string> dateDict = new Dictionary<Language, string>();

        private readonly Dictionary<Language, string> weekDayDict = new Dictionary<Language, string>();

        private readonly Dictionary<Language, string> dayTimeDict = new Dictionary<Language, string>();

        private readonly Dictionary<Language, string> descriptionDict = new Dictionary<Language, string>();

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public Language Language { get; private set; }

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

        public string Date
        {
            get
            {
                return dateDict.Count != 0 ? dateDict[Language] : null ;
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

        public string Longitude { get; private set; }

        public string Latitude { get; private set; }

        public int Pressure { get; private set; }

        public int Humidity { get; private set; }

        public int WindSpeed { get; private set; }

        public int WindAngle { get; private set; }

        public string Icon { get; private set; }

        public string WeekDay
        {
            get
            {
                return  weekDayDict.Count != 0 ? weekDayDict[Language] : null;
            }
        }

        #endregion

        #region Constructors

        public HourlyForecastTransferObject(string description, double avgTemp, double feelTemp, double snowFall, double rainFall, int pressure, int humidity, double windSpeed,
            int windAngle, string icon, string country = null, string cityName = null, string cityId = null, long? sunsetTime = null, long? sunriseTime = null,
            double? longitude = null, double? latitude = null, string descriptionId = null, DateTime? date = null, long? dayTime = null)
        {
            PrepareDictionaries(Language.Polish, description, descriptionId, date, dayTime);
            PrepareDictionaries(Language.English, description, descriptionId, date, dayTime);

            Language = Language.Polish;
            DescriptionId = descriptionId;
            CityName = cityName;
            CityId = cityId;
            AvgTemp = Convert.ToInt32(avgTemp);
            FeelTemp = Convert.ToInt32(feelTemp);
            SnowFall = Math.Round(Convert.ToDouble(snowFall), 1);
            RainFall = Math.Round(Convert.ToDouble(rainFall), 1);
            WindAngle = Convert.ToInt32(windAngle - 180);
            Icon = icon;
            SunsetTime = sunsetTime != null ? (Utils.UnixTimeStampToDateTime(sunsetTime.Value) + DateTimeOffset.Now.Offset).ToString("HH:mm") : null;
            SunriseTime = sunriseTime != null ? (Utils.UnixTimeStampToDateTime(sunriseTime.Value) + DateTimeOffset.Now.Offset).ToString("HH:mm") : null;
            Longitude = longitude != null ? Utils.ConvertCoordinatesFromDecToDeg(longitude.Value, true) : null;
            Latitude = latitude != null ? Utils.ConvertCoordinatesFromDecToDeg(latitude.Value, false) : null;
            Pressure = Convert.ToInt32(pressure);
            Humidity = Convert.ToInt32(humidity);
            Country = country;
            WindSpeed = Convert.ToInt32(windSpeed * 3.6);
        }

        private HourlyForecastTransferObject()
        {
            Icon = "01d";
            DescriptionId = "800";
        }

        #endregion

        #region Public methods

        public static HourlyForecastTransferObject Empty()
        {
            return new HourlyForecastTransferObject();
        }

        public void ChangeLanguage(Language language)
        {
            Language = language;

            OnPropertyChanged("Description");
            OnPropertyChanged("Date");
            OnPropertyChanged("DayTime");
            OnPropertyChanged("WeekDay");
        }

        #endregion

        #region Private methods

        private void PrepareDictionaries(Language language, string description, string descriptionId, DateTime? date, long? dayTime)
        {
            var cultureName = new CultureInfo(language == Language.English ? "en-US" : "pl-PL");

            if (dayTime == null)
            {
                dayTimeDict.Add(language, language == Language.Polish ? "Teraz" : "Now");
            }
            else
            {
                dayTimeDict.Add(language, (Utils.UnixTimeStampToDateTime(dayTime.Value) + DateTimeOffset.Now.Offset).ToString("HH:mm"));
            }

            if (language == Language.Polish)
            {
                descriptionDict.Add(language, description.FirstOrDefault().ToString().ToUpper() + description.Substring(1));
            }
            else
            {
                descriptionDict.Add(language, descriptionId != null ? Utils.GetDescriptionFromId(descriptionId) : string.Empty);
            }

            if (date != null)
            {
                weekDayDict.Add(language, cultureName.DateTimeFormat.GetDayName(date.Value.DayOfWeek).FirstOrDefault().ToString().ToUpper() +
                        cultureName.DateTimeFormat.GetDayName(date.Value.DayOfWeek).Substring(1));
                dateDict.Add(language, date.Value.ToString("dd MMMM", cultureName).First() == '0' ? date.Value.ToString("dd MMMM", cultureName).Remove(0, 1) : date.Value.ToString("dd MMMM", cultureName));
            }
        }

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
