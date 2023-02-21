using System;
using System.Configuration;
using WeatherBar.Application.Services.Interfaces;
using WeatherBar.Model;
using WeatherBar.Model.Enums;

namespace WeatherBar.Application.Services
{
    public class ConfigurationService : IConfigurationService
    {
        #region Public methods

        public AppSettings GetAppSettings()
        {
            string[] apiKeysArray = ConfigurationManager.AppSettings.Get("ApiKeys").Replace(" ", string.Empty).Split(',');

            return new AppSettings(apiKeysArray[new Random().Next(0, apiKeysArray.Length)],
                ConfigurationManager.AppSettings.Get("CityId"),
                int.Parse(ConfigurationManager.AppSettings.Get("Interval")),
                (Units)Enum.Parse(typeof(Units), ConfigurationManager.AppSettings.Get("Units")),
                (Language)Enum.Parse(typeof(Language), ConfigurationManager.AppSettings.Get("Language")))
            {
            };
        }

        public void UpdateAndSaveConfiguration(AppSettings appSettings)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings["Language"].Value = appSettings.Language.ToString();
            config.AppSettings.Settings["Units"].Value = appSettings.Units.ToString();
            config.AppSettings.Settings["Interval"].Value = appSettings.Interval.ToString();
            config.AppSettings.Settings["CityId"].Value = appSettings.CityId;

            config.Save(ConfigurationSaveMode.Modified, true);
        }

        #endregion
    }
}
