using AppResources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WeatherBar.Core.Commands;
using WeatherBar.Core.Enums;
using WeatherBar.Core.Events;
using WeatherBar.Core.Events.Args;
using WeatherBar.Model.Enums;
using WeatherBar.Model.Interfaces;
using WeatherBar.Services;
using WeatherBar.Services.Interfaces;
using WeatherBar.Utils;
using WeatherBar.ViewModel.Templates;

namespace WeatherBar.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private readonly ICityDataService cityDataService;

        private readonly IWeatherDataService weatherDataService;

        private IHourlyData currentWeatherData;

        private IFourDaysData weatherForecastData;

        private EventDispatcher autoUpdateEvent;

        private AppStatus appStatus;

        private bool isOptionsPanelVisible;

        private bool isForecastPanelVisible;

        private DateTime? dailyForecastDate;

        private List<IDailyData> fourDaysForecast;

        private Tuple<List<IHourlyData>, List<IHourlyData>> hourlyForecast;

        #endregion

        #region Public properties

        public ICommand CloseCommand { get; private set; }

        public ICommand OpenSiteCommand { get; private set; }

        public ICommand ShowOptionsCommand { get; private set; }

        public ICommand ShowForecastCommand { get; private set; }

        public ICommand RefreshCommand { get; private set; }

        public AppStatus AppStatus
        {
            get => appStatus;
            set
            {
                appStatus = value;
                Notify();
            }
        }

        public bool IsOptionsPanelVisible
        {
            get => isOptionsPanelVisible;
            set
            {
                isOptionsPanelVisible = value;
                Notify();
            }
        }

        public bool IsForecastPanelVisible
        {
            get => isForecastPanelVisible;
            set
            {
                isForecastPanelVisible = value;
                Notify();
            }
        }

        public List<IDailyData> FourDaysForecast
        {
            get => fourDaysForecast;
            private set
            {
                fourDaysForecast = value;
                Notify();
            }
        }

        public Tuple<List<IHourlyData>, List<IHourlyData>> HourlyForecast
        {
            get => hourlyForecast;
            private set
            {
                hourlyForecast = value;
                Notify();
            }
        }

        public Tuple<BitmapImage, Color> BackgroundImage
        {
            get
            {
                var imageData = ResourceManager.GetImageWithHexColor(CurrentWeatherData.Icon, CurrentWeatherData.FeelTemp, CurrentWeatherData.Description);

                return new Tuple<BitmapImage, Color>(
                    ViewModelUtils.LoadImage(imageData.Key), (Color)ColorConverter.ConvertFromString(imageData.Value));
            }
        }

        #endregion

        #region Private properties

        private IHourlyData CurrentWeatherData
        {
            get
            {
                if (currentWeatherData == null)
                {
                    currentWeatherData = Model.HourlyForecast.Empty();
                }

                return currentWeatherData;
            }
            set
            {
                currentWeatherData = value;
                Notify();
            }
        }

        private bool UpdateConfiguration { get; set; }

        private EventDispatcher AutoUpdateEvent
        {
            get
            {
                if (autoUpdateEvent == null)
                {
                    autoUpdateEvent = new EventDispatcher(AutoRefresh, (RefreshTime)App.AppSettings.Interval, true);
                }

                return autoUpdateEvent;
            }
        }

        #endregion

        #region Constructors

        public MainWindowViewModel()
        {
            this.AppStatus = AppStatus.Starting;
            this.AutomaticallyApplyReceivedChanges = true;
            this.ReceiveOnlyPublicChanges = false;
            this.SendOnlyPublicChanges = false;
            this.MessageReceived += MainWindowViewModel_MessageReceived;
            this.weatherDataService = new WeatherDataService();
            this.cityDataService = new CityDataService();
            this.IsOptionsPanelVisible = false;
            this.IsForecastPanelVisible = false;
            this.ShowForecastCommand = new RelayCommand(ShowForecast, (o) => o != null && (int)o != -1);
            this.CloseCommand = new RelayCommand(SaveConfiguration);
            this.OpenSiteCommand = new RelayCommand(OpenWeathermapSite);
            this.ShowOptionsCommand = new RelayCommand(ShowOptions);
            this.RefreshCommand = new RelayCommand(Refresh);

            GetAndUpdateWeatherData(App.AppSettings.CityId);
        }

        #endregion

        #region Public methods

        public void Refresh()
        {
            GetAndUpdateWeatherData(CurrentWeatherData.CityId ?? App.AppSettings.CityId);
        }

        #endregion

        #region Private methods

        private void MainWindowViewModel_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            switch (e.CallerName)
            {
                case "UpdateWeatherData":
                    var input = (string)e.Message;

                    GetAndUpdateWeatherData(input);
                    break;
                case "ShowDailyForecast":
                    dailyForecastDate = (DateTime)e.Message;

                    UpdateDailyForecast();
                    break;
                case "TryRefreshAgain":
                    Refresh();
                    break;
                case "RefreshTimeUpdated":
                    UpdateRefreshTime((RefreshTime)e.Message);
                    break;
                case "UnitsUpdated":
                    ExecuteIfAppStarted(UpdateUnits);
                    break;
                case "LanguageUpdated":
                    ExecuteIfAppStarted(UpdateLanguage);
                    break;
            }
        }

        private void UpdateDailyForecast()
        {
            if (dailyForecastDate != null)
            {
                Notify("DailyForecastChanged",
                    ViewModelUtils.GetHourlyForecastForSpecificDate(weatherForecastData.HourlyData, App.AppSettings.Language, dailyForecastDate.GetValueOrDefault()));
            }
        }

        private void ShowForecast(object obj)
        {
            Notify("ShowDailyForecast", DateTime.Now.AddDays((int)obj + 1));

            this.IsForecastPanelVisible = true;
        }

        private void AutoRefresh()
        {
            GetAndUpdateWeatherData(CurrentWeatherData.CityId ?? App.AppSettings.CityId);
        }

        private void GetAndUpdateWeatherData(string input)
        {
            Task.Run(() => TryGetWeatherData(input));
            AutoUpdateEvent.Restart();
        }

        private void TryGetWeatherData(string cityData)
        {
            try
            {
                ExecuteIfAppStarted(() => AppStatus = AppStatus.LoadingResource);

                System.Threading.Thread.Sleep(AppStatus != AppStatus.Starting ? 500 : 2000);

                weatherForecastData = weatherDataService.GetFourDaysData(cityData);
                CurrentWeatherData = weatherDataService.GetHourlyData(cityData);
                HourlyForecast = ViewModelUtils.GetHourlyForecastFromHourlyData(weatherForecastData.HourlyData);
                FourDaysForecast = weatherForecastData.DailyData;

                UpdateUnits();
                UpdateLanguage();
                SetStartlingLocation();
                UpdateProperties();

                AppStatus = AppStatus.Ready;
            }
            catch (HttpException)
            {
                AppStatus = AppStatus.ResourceNotFound;
            }
            catch (TaskCanceledException)
            {
                AppStatus = AppStatus.ConnectionFailed;
            }
        }

        private void ShowOptions(object obj)
        {
            this.IsOptionsPanelVisible = !this.IsOptionsPanelVisible;
        }

        private void SetStartlingLocation()
        {
            if (AppStatus == AppStatus.Starting)
            {
                Notify("SetStartingLocation", cityDataService.GetCityById(currentWeatherData.CityId));
            }
        }

        private void OpenWeathermapSite()
        {
            Process.Start($"https://openweathermap.org/");
        }

        private void UpdateProperties()
        {
            var propertiesToUpdate = new List<string>()
                {
                    "BackgroundImage",
                    "WeatherDataUpdated"
                };

            propertiesToUpdate.ForEach(property => Notify(property));
        }

        private void UpdateRefreshTime(RefreshTime refreshTime)
        {
            AutoUpdateEvent.UpdateInterval(refreshTime);
        }

        private void UpdateUnits()
        {
            Units units = App.AppSettings.Units;

            ViewModelUtils.GetXmlResource(units).Use();

            weatherForecastData = weatherForecastData.Clone();

            weatherForecastData.ChangeUnits(units);

            HourlyForecast = ViewModelUtils.GetHourlyForecastFromHourlyData(weatherForecastData.HourlyData);
            FourDaysForecast = weatherForecastData.DailyData;

            CurrentWeatherData.ChangeUnits(units);
            Notify("CurrentWeatherData");
            UpdateDailyForecast();
        }

        private void UpdateLanguage()
        {
            Language language = App.AppSettings.Language;

            ViewModelUtils.GetXmlResource(language).Use();

            weatherForecastData = weatherForecastData.Clone();

            weatherForecastData.ChangeLanguage(language);

            HourlyForecast = ViewModelUtils.GetHourlyForecastFromHourlyData(weatherForecastData.HourlyData);
            FourDaysForecast = weatherForecastData.DailyData;

            CurrentWeatherData.ChangeLanguage(language);
            UpdateDailyForecast();
        }

        private void SaveConfiguration()
        {
            if (UpdateConfiguration)
            {
                App.UpdateAndSaveConfiguration();
            }
        }

        private void ExecuteIfAppStarted(Action action)
        {
            if (AppStatus != AppStatus.Starting)
            {
                action?.Invoke();
            }
        }

        #endregion
    }
}
