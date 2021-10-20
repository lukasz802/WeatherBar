using System.Windows;
using WeatherBar.Core;
using WeatherBar.ViewModel;
using WebApi;

namespace WeatherBar
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        private static AppViewModel viewModel;

        private static IWeatherApi apiClient;

        private static AppSettings appSettings;

        #endregion

        #region Properties

        public static IWeatherApiClient ApiClient => apiClient ?? (apiClient = InitializeAndConfigureApiClient());

        public static AppViewModel ViewModel => viewModel ?? (viewModel = new AppViewModel());

        public static AppSettings AppSettings => appSettings ?? (appSettings = new AppSettings());

        #endregion

        #region Public methods

        public static void UpdateAndSaveConfiguration()
        {
            AppSettings.UpdateAndSaveConfiguration();
            apiClient.UpdateConfiguration(cityId: AppSettings.CityId, interval: AppSettings.Interval);
        }

        #endregion

        #region Private methods

        private static IWeatherApi InitializeAndConfigureApiClient()
        {
            return new WeatherApi(apiKey: AppSettings.ApiKey, interval: AppSettings.Interval, cityId: AppSettings.CityId);
        }

        #endregion
    }
}
