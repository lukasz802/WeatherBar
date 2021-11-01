using System;
using System.Configuration;
using WeatherBar.Model.Enums;

namespace WeatherBar.Core
{
    public class AppSettings
    {
        #region Fields

        private int interval;

        #endregion

        #region Properties

        public string ApiKey { get; private set; }

        public Units Units { get; set; }

        public string CityId { get; set; }

        public int Interval
        {
            get => interval;
            set
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

        #region Constructor

        public AppSettings()
        {
            GetAppSettings();
        }

        #endregion

        #region Public methods

        public void UpdateAndSaveConfiguration()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings["Language"].Value = Language.ToString();
            config.AppSettings.Settings["Units"].Value = Units.ToString();
            config.AppSettings.Settings["Interval"].Value = Interval.ToString();
            config.AppSettings.Settings["CityId"].Value = CityId.ToString();

            config.Save(ConfigurationSaveMode.Modified, true);
        }

        #endregion

        #region Private methods

        private void GetAppSettings()
        {
            string[] apiKeysArray = ConfigurationManager.AppSettings.Get("ApiKeys").Replace(" ", string.Empty).Split(',');

            Language = (Language)Enum.Parse(typeof(Language), ConfigurationManager.AppSettings.Get("Language"));
            ApiKey = apiKeysArray[new Random().Next(0, apiKeysArray.Length)];
            Units = (Units)Enum.Parse(typeof(Units), ConfigurationManager.AppSettings.Get("Units"));
            Interval = int.Parse(ConfigurationManager.AppSettings.Get("Interval"));
            CityId = ConfigurationManager.AppSettings.Get("CityId");
        }

        #endregion
    }
}
