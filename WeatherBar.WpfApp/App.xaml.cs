using System.Collections.Generic;
using WeatherBar.Application.Services;
using WeatherBar.Application.Services.Interfaces;
using WeatherBar.Model;
using WeatherBar.Model.Enums;

namespace WeatherBar.WpfApp
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        #region Fields

        private static readonly IConfigurationService configurationService = new ConfigurationService();

        private static readonly AppSettings initAppSettings = configurationService.GetAppSettings();

        #endregion

        #region Properties

        public static AppSettings AppSettings { get; private set; } = new AppSettings(initAppSettings);

        #endregion

        #region Public methods

        public static void SaveConfiguration()
        {
            if (!AppSettings.Equals(initAppSettings))
            {
                configurationService.UpdateAndSaveConfiguration(AppSettings);
            }
        }

        public static void UpdateConfiguration(Units units)
        {
            AppSettings.Update(units);
        }

        public static void UpdateConfiguration(Language language)
        {
            AppSettings.Update(language);
        }

        public static void UpdateConfiguration(RefreshTime refreshTime)
        {
            AppSettings.Update((int)refreshTime);
        }

        public static void UpdateConfiguration(string cityId)
        {
            AppSettings.Update(cityId);
        }

        public static void UpdateResources(Dictionary<string, string> resourceDictionary)
        {
            foreach (string key in resourceDictionary.Keys)
            {
                if (Current.Resources.Contains(key))
                {
                    Current.Resources[key] = resourceDictionary[key];
                }
            }
        }

        #endregion
    }
}
