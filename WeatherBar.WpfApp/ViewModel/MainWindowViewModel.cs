using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WeatherBar.Application.Commands;
using WeatherBar.Application.Dispatchers;
using WeatherBar.Application.Events;
using WeatherBar.Application.Events.Interfaces;
using WeatherBar.AppResources;
using WeatherBar.DataProviders;
using WeatherBar.DataProviders.Interfaces;
using WeatherBar.Model;
using WeatherBar.Model.Enums;
using WeatherBar.WpfApp.Controls.WinForms;
using WeatherBar.WpfApp.ViewModel.Templates;

namespace WeatherBar.WpfApp.ViewModel
{
    public class MainWindowViewModel : ViewModelBase,
        IEventHandler<AppStatusChangedEvent>,
        IEventHandler<WeatherDataUpdatedEvent>, 
        IEventHandler<LanguageUpdatedEvent>, 
        IEventHandler<ShowDailyForecastEvent>, 
        IEventHandler<ForecastPanelVisibilityChangedEvent>,
        IEventHandler<UnitsUpdatedEvent>,
        IEventHandler<RefreshTimeUpdatedEvent>,
        IEventHandler<StartingLocationUpdatedEvent>,
        IEventHandler<WeatherDataRefreshedEvent>
    {
        #region Fields

        private readonly ICityDataProvider cityDataProvider;

        private readonly IWeatherDataProvider weatherDataProvider;

        private HourlyForecast currentWeatherData = Model.HourlyForecast.Empty();

        private FourDaysForecast fourDaysWeatherData;

        private EventDispatcher autoUpdateEvent;

        private AppStatus appStatus;

        private bool isOptionsPanelVisible;

        private bool isForecastPanelVisible;

        private int feelTempForBackgroundImage;

        #endregion

        #region Public properties

        public ICommand CloseCommand { get; private set; }

        public ICommand OpenSiteCommand { get; private set; }

        public ICommand ShowOptionsCommand { get; private set; }

        public ICommand RefreshCommand { get; private set; }

        public AppStatus AppStatus
        {
            get => appStatus;
            set
            {
                appStatus = value;
                Notify(new AppStatusChangedEvent(this, value));
                UpdateTrayNotifyIcon(value);
            }
        }

        public bool IsOptionsPanelVisible
        {
            get => isOptionsPanelVisible;
            set
            {
                isOptionsPanelVisible = value;
                Notify(new OptionsPanelVisibilityChangedEvent(this, value));
            }
        }

        public bool IsForecastPanelVisible
        {
            get => isForecastPanelVisible;
            set
            {
                isForecastPanelVisible = value;
                Notify(new ForecastPanelVisibilityChangedEvent(this, value));
            }
        }

        public List<DailyForecast> FourDaysForecast { get; private set; }

        public Tuple<IEnumerable<HourlyForecast>, IEnumerable<HourlyForecast>> HourlyForecast { get; private set; }

        public KeyValuePair<BitmapImage, Color> BackgroundImage
        {
            get
            {
                return ResourceManager.GetImageWithColor(currentWeatherData.Icon, feelTempForBackgroundImage, currentWeatherData.Description);
            }
        }

        #endregion

        #region Private properties

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
            this.weatherDataProvider = new WeatherDataProvider(App.AppSettings.ApiKey);
            this.cityDataProvider = new CityDataProvider();
            this.IsOptionsPanelVisible = false;
            this.IsForecastPanelVisible = false;
            this.CloseCommand = new RelayCommand(SaveConfiguration);
            this.OpenSiteCommand = new RelayCommand(OpenWeathermapSite);
            this.ShowOptionsCommand = new RelayCommand(ShowOptions);
            this.RefreshCommand = new RelayCommand(RefreshWeatherData);

            RefreshWeatherData();
        }

        #endregion

        #region Public methods

        public void RefreshWeatherData()
        {
            GetAndUpdateWeatherData(currentWeatherData.CityId ?? App.AppSettings.CityId);
        }

        public void Handle(AppStatusChangedEvent @event)
        {
            AppStatus = @event.Content;
        }

        public void Handle(ForecastPanelVisibilityChangedEvent @event)
        {
            IsForecastPanelVisible = @event.Content;
        }

        public void Handle(WeatherDataUpdatedEvent @event)
        {
            GetAndUpdateWeatherData(@event.Content);
        }

        public void Handle(WeatherDataRefreshedEvent @event)
        {
            RefreshWeatherData();
        }

        public void Handle(ShowDailyForecastEvent @event)
        {
            Notify(new DailyForecastUpdatedEvent(this,
                GetHourlyForecastForSpecificDate(fourDaysWeatherData.HourlyData, App.AppSettings.Language, @event.Content)));
        }

        public void Handle(RefreshTimeUpdatedEvent @event)
        {
            App.UpdateConfiguration(@event.Content);
            AutoUpdateEvent.UpdateInterval(@event.Content);
        }

        public void Handle(UnitsUpdatedEvent @event)
        {
            App.UpdateConfiguration(@event.Content);
            UpdateUnits();
            UpdateMainPanel(fourDaysWeatherData, currentWeatherData);
        }

        public void Handle(LanguageUpdatedEvent @event)
        {
            App.UpdateConfiguration(@event.Content);
            UpdateLanguage();
            UpdateMainPanel(fourDaysWeatherData, currentWeatherData);
        }

        public void Handle(StartingLocationUpdatedEvent @event)
        {
            var cityId = @event.Content.Id.ToString();

            App.UpdateConfiguration(cityId);
            GetAndUpdateWeatherData(cityId);
        }

        #endregion

        #region Private methods

        private void AutoRefresh()
        {
            GetAndUpdateWeatherData(currentWeatherData.CityId);
        }

        private void GetAndUpdateWeatherData(string input)
        {
            Task.Run(() => TryGetAndUpdateWeatherData(input));
            AutoUpdateEvent.Restart();
        }

        private void TryGetAndUpdateWeatherData(string cityData)
        {
            try
            {
                ChangeStatusToLoadingResourceIfAppStarted();
                HideForecastPanelIfVisible();
                ChangeStatusWithDelay(AppStatus);

                fourDaysWeatherData = weatherDataProvider.GetFourDaysForecast(cityData);
                currentWeatherData = weatherDataProvider.GetCurrentForecast(cityData);
                feelTempForBackgroundImage = currentWeatherData.FeelTemp;

                SetStartingLocation();
                UpdateUnits();
                UpdateLanguage();
                UpdateMainPanel(fourDaysWeatherData, currentWeatherData);
                ChangeStatusWithDelay(AppStatus.Ready);
            }
            catch (HttpException)
            {
                ChangeStatusWithDelay(AppStatus.ResourceNotFound);
            }
            catch (TaskCanceledException)
            {
                ChangeStatusWithDelay(AppStatus.ConnectionFailed);
            }
        }

        private void UpdateTrayNotifyIcon(AppStatus appStatus)
        {
            if (appStatus == AppStatus.Starting || appStatus == AppStatus.LoadingResource)
            {
                TrayNotifyIcon.Instance.Update((string)App.Current.Resources["Updating"], "Update");
            }
            else if (appStatus == AppStatus.ConnectionFailed)
            {
                TrayNotifyIcon.Instance.Update((string)App.Current.Resources["NoConnectionServer"], string.Empty);
            }
            else
            {
                TrayNotifyIcon.Instance.Update($"{currentWeatherData.CityName}, {currentWeatherData.Country}\n{currentWeatherData.Description}\n{(string)App.Current.Resources["Temperature"]} " +
                   $"{currentWeatherData.AvgTemp}/{currentWeatherData.FeelTemp}{(string)App.Current.Resources["TempUnit"]}\n{(string)App.Current.Resources["Update"]} {currentWeatherData.UpdateTime}",
                   currentWeatherData.Icon);
            }
        }

        private void ShowOptions(object obj)
        {
            this.IsOptionsPanelVisible = !this.IsOptionsPanelVisible;
        }

        private void UpdateMainPanel(FourDaysForecast fourDaysWeatherData, HourlyForecast currentWeatherData)
        {
            HourlyForecast = GetHourlyForecastFromHourlyData(fourDaysWeatherData.HourlyData);
            FourDaysForecast = fourDaysWeatherData.DailyData;

            Notify(new CurrentWeatherDataUpdatedEvent(this, currentWeatherData));
            Notify(new FourDaysForecastDataUpdatedEvent(this, fourDaysWeatherData));
        }

        private void OpenWeathermapSite()
        {
            Process.Start($"https://openweathermap.org/");
        }

        private void UpdateUnits()
        {
            Units units = App.AppSettings.Units;

            App.UpdateResources(ResourceManager.GetUnits(units));

            fourDaysWeatherData.ChangeUnits(units);
            currentWeatherData.ChangeUnits(units);
        }

        private void UpdateLanguage()
        {
            Language language = App.AppSettings.Language;

            App.UpdateResources(ResourceManager.GetLanguage(language));

            fourDaysWeatherData.ChangeLanguage(language);
            currentWeatherData.ChangeLanguage(language);
        }

        private void SaveConfiguration()
        {
            App.SaveConfiguration();
        }

        private void ChangeStatusToLoadingResourceIfAppStarted()
        {
            if (AppStatus != AppStatus.Starting)
            {
                AppStatus = AppStatus.LoadingResource;
            }
        }

        private void SetStartingLocation()
        {
            if (AppStatus == AppStatus.Starting)
            {
                Notify(new StartingLocationUpdatedEvent(this, cityDataProvider.GetCityById(currentWeatherData.CityId)));
            }
        }

        private void ChangeStatusWithDelay(AppStatus appStatus)
        {
            switch (appStatus)
            {
                case AppStatus.Ready:
                case AppStatus.LoadingResource:
                    System.Threading.Thread.Sleep(500);
                    break;
                case AppStatus.Starting:
                case AppStatus.ResourceNotFound:
                case AppStatus.ConnectionFailed:
                    System.Threading.Thread.Sleep(2000);
                    break;
            }

            if (AppStatus != appStatus)
            {
                AppStatus = appStatus;
            }
        }

        private void HideForecastPanelIfVisible()
        {
            if (IsForecastPanelVisible)
            {
                IsForecastPanelVisible = false;
            }
        }

        private List<HourlyForecast> GetHourlyForecastForSpecificDate(List<HourlyForecast> hourlyData, Language language, DateTime date)
        {
            var cultureName = new CultureInfo(language == Language.English ? "en-US" : "pl-PL");
            var tempDate = date.ToString("dd MMMM", cultureName).Trim();

            return hourlyData.Where(x => x.Date.Contains(tempDate.First() == '0' ? tempDate.Remove(0, 1) : tempDate)).ToList();
        }

        private Tuple<IEnumerable<HourlyForecast>, IEnumerable<HourlyForecast>> GetHourlyForecastFromHourlyData(IEnumerable<HourlyForecast> hourlyData)
        {
            return new Tuple<IEnumerable<HourlyForecast>, IEnumerable<HourlyForecast>>(hourlyData.Take(5), hourlyData.ToList().GetRange(5, 5));
        }

        #endregion
    }
}
