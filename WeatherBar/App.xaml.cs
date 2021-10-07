using System;
using System.Configuration;
using System.Windows;
using WeatherBar.ViewModel;
using WebApi;
using WebApi.Model.Enums;

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

        private static int interval;

        #endregion

        #region Properties

        public static string ApiKey { get; private set; }

        public static Units Units { get; set; }

        public static string CityId { get; set; }

        public static int Interval
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

        public static Language Language { get; set; }

        #endregion

        #region Constructor

        public App() : base()
        {
            GetAppSettings();
            apiClient = InitializeAndConfigureApiClient();
        }

        #endregion

        #region Properties

        public static AppViewModel ViewModel
        {
            get
            {
                if (viewModel == null)
                {
                    viewModel = new AppViewModel(apiClient);
                }

                return viewModel;
            }
        }

        #endregion

        #region Public methods

        public static void UpdateAndSaveConfiguration()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings["Language"].Value = Language.ToString();
            config.AppSettings.Settings["Units"].Value = Units.ToString();
            config.AppSettings.Settings["Interval"].Value = Interval.ToString();
            config.AppSettings.Settings["CityId"].Value = CityId.ToString();
            apiClient.UpdateConfiguration(cityId: CityId, units: Units, interval: Interval);

            config.Save(ConfigurationSaveMode.Modified, true);
        }

        #endregion

        #region Private methods

        private static IWeatherApi InitializeAndConfigureApiClient()
        {
            return new WeatherApi(apiKey: ApiKey, units: Units, interval: interval, cityId: CityId);
        }

        private static void GetAppSettings()
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
