using System;
using WeatherBar.Model.Enums;

namespace WeatherBar.Model
{
    public class AppSettings : IEquatable<AppSettings>
    {
        #region Fields

        private int interval;

        #endregion

        #region Properties

        public string ApiKey { get; private set; }

        public Units Units { get; private set; }

        public string CityId { get; private set; }

        public int Interval
        {
            get => interval;
            private set
            {
                if (value < 15)
                {
                    interval = 15;
                }
                else if (value > 60)
                {
                    interval = 60;
                }
                else
                {
                    interval = value;
                }
            }
        }

        public Language Language { get; set; }

        #endregion

        #region Constructors

        public AppSettings(string apiKey, string cityId, int interval, Units units, Language language)
        {
            ApiKey = apiKey;
            CityId = cityId;
            Units = units;
            Language = language;
            Interval = interval;
        }

        public AppSettings(AppSettings appSettings)
        {
            ApiKey = appSettings.ApiKey;
            CityId = appSettings.CityId;
            Units = appSettings.Units;
            Language = appSettings.Language;
            Interval = appSettings.Interval;
        }

        #endregion

        #region Public methods

        public bool Equals(AppSettings other)
        {
            if (other == null)
            {
                return false;
            }

            return (CityId, Units, Language, Interval).Equals((other.CityId, other.Units, other.Language, other.Interval));
        }

        #endregion
    }
}
