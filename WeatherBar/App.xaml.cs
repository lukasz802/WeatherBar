using System.Windows;
using WeatherBar.Core;
using WeatherDataProvider.Interfaces;

namespace WeatherBar
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        private static IWeatherDataProvider weatherDataProvider;

        private static AppSettings appSettings;

        #endregion

        #region Properties

        public static IWeatherDataProvider WeatherDataProvider => weatherDataProvider ?? (weatherDataProvider = InitializeAndConfigureWeatherDataProvider());

        public static AppSettings AppSettings => appSettings ?? (appSettings = new AppSettings());

        #endregion

        #region Public methods

        public static void UpdateAndSaveConfiguration()
        {
            AppSettings.UpdateAndSaveConfiguration();
        }

        #endregion

        #region Private methods

        private static IWeatherDataProvider InitializeAndConfigureWeatherDataProvider()
        {
            return new WeatherDataProvider.WeatherDataProvider(apiKey: AppSettings.ApiKey);
        }

        #endregion
    }
}
