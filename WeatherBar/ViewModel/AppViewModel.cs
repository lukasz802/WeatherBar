using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using WeatherBar.Core;
using System.Diagnostics;
using Microsoft.Rest;
using WebApi.Model.Enums;
using WeatherBar.Utils;
using WeatherBar.Model.DataTransferObjects;
using AppResources;
using WeatherBar.Model.Enums;
using WeatherBar.Model.Interfaces;
using WeatherBar.Model.Services;
using WeatherBar.Model;
using WeatherBar.Model.Services.Interfaces;

namespace WeatherBar.ViewModel
{
    public class AppViewModel : INotifyPropertyChanged
    {
        #region Fields

        private static AppViewModel instance;

        private readonly ICityDataService cityDataService;

        private readonly IWeatherDataService weatherDataService;

        private EventDispatcher autoUpdateEvent;

        private IHourlyData currentWeatherData;

        private IFourDaysData weatherForecastData;

        private CallType refreshCallTypeMethod;

        private Language appLanguage;

        private Units appUnits;

        private RefreshTime refreshTime;

        private City startingLocation;

        private bool isReady;

        private bool isConnected;

        private bool hasStarted;

        private bool resourceFounded;

        private bool isForecastPanelVisible;

        private bool isOptionsPanelVisible;

        private bool updateConfiguration = false;

        private string searchText;

        private QueryExecutionTransferObject queryResult;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public properties

        public static AppViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppViewModel();
                }

                return instance;
            }
        }

        public ICommand CloseCommand { get; private set; }

        public ICommand ShowMapCommand { get; private set; }

        public ICommand OpenSiteCommand { get; private set; }

        public ICommand RefreshCommand { get; private set; }

        public ICommand SearchCommand { get; private set; }

        public ICommand QueryCommand { get; private set; }

        public ICommand StartingLocationCommand { get; private set; }

        public ICommand QueryResultCommand { get; private set; }

        public ICommand StartingLocationResultCommand { get; private set; }

        public ICommand ShowForecastCommand { get; private set; }

        public ICommand ReturnToMainPanelCommand { get; private set; }

        public ICommand ShowOptionsCommand { get; private set; }

        public ICommand SelectLanguageCommand { get; private set; }

        public ICommand SelectUnitsCommand { get; private set; }

        public ICommand RefreshTimeCommand { get; private set; }

        public Language ApplicationLanguage
        {
            get => appLanguage;
            private set
            {
                if (value != appLanguage)
                {
                    appLanguage = value;
                    OnApplicationLanguageChanged(value);
                }
            }
        }

        public RefreshTime RefreshTime
        {
            get => refreshTime;
            private set
            {
                if (value != refreshTime)
                {
                    refreshTime = value;
                    OnRefreshTimeChanged(value);
                }
            }
        }

        public Units ApplicationUnits
        {
            get => appUnits;
            private set
            {
                if (value != appUnits)
                {
                    appUnits = value;
                    OnApplicationUnitsChanged(value);
                }
            }
        }

        public City StartingLocation
        {
            get => startingLocation;
            private set
            {
                if (value != startingLocation)
                {
                    startingLocation = value;
                    OnStartinglocationChanged(value);
                }
            }
        }

        public bool IsReady
        {
            get => isReady;
            private set
            {
                isReady = value;
                OnPropertyChanged();
            }
        }

        public bool HasStarted
        {
            get => hasStarted;
            private set
            {
                hasStarted = value;
                OnPropertyChanged();
            }
        }

        public bool ResourceFounded
        {
            get => resourceFounded;
            private set
            {
                resourceFounded = value;
                OnPropertyChanged();
            }
        }

        public bool IsConnected
        {
            get => isConnected;
            private set
            {
                isConnected = value;
                OnPropertyChanged();
            }
        }

        public bool IsForecastPanelVisible
        {
            get => isForecastPanelVisible;
            set
            {
                isForecastPanelVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsOptionsPanelVisible
        {
            get => isOptionsPanelVisible;
            set
            {
                isOptionsPanelVisible = value;
                OnPropertyChanged();
            }
        }

        public string CityName => currentWeatherData.CityName;

        public string CityId => currentWeatherData.CityId?.ToString();

        public string Description => currentWeatherData.Description;

        public string DescriptionId => currentWeatherData.DescriptionId;

        public int AvgTemp => currentWeatherData.AvgTemp;

        public int FeelTemp => currentWeatherData.FeelTemp;

        public double SnowFall => currentWeatherData.SnowFall;

        public double RainFall => currentWeatherData.RainFall;

        public string UpdateTime => DateTime.Now.ToString("HH:mm");

        public string SunsetTime => currentWeatherData.SunsetTime;

        public string SunriseTime => currentWeatherData.SunriseTime;

        public string Country => currentWeatherData.Country;

        public string Longitude => currentWeatherData.Longitude;

        public string Latitude => currentWeatherData.Latitude;

        public int Pressure => currentWeatherData.Pressure;

        public int Humidity => currentWeatherData.Humidity;

        public int WindSpeed => currentWeatherData.WindSpeed;

        public int WindAngle => currentWeatherData.WindAngle;

        public string Icon => currentWeatherData.Icon;

        public KeyValuePair<BitmapImage, Color> BackgroundImage
        {
            get
            {
                var imageData = ResourceManager.GetImageWithHexColor(Icon, FeelTemp, Description);

                return new KeyValuePair<BitmapImage, Color>(
                    ViewModelUtils.LoadImage(imageData.Key), (Color)ColorConverter.ConvertFromString(imageData.Value));
            }
        }

        public List<IDailyData> FourDaysForecast { get; private set; }

        public Tuple<IEnumerable<IHourlyData>, IEnumerable<IHourlyData>> HourlyForecast { get; private set; }

        public List<IHourlyData> DailyForecast { get; private set; }

        public QueryExecutionTransferObject QueryResult
        {
            get => queryResult;
            private set
            {
                queryResult = value;
                OnPropertyChanged();
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
                    autoUpdateEvent = new EventDispatcher(AutoRefresh, RefreshTime, true);
                }

                return autoUpdateEvent;
            }
        }

        #endregion

        #region Constructors

        private AppViewModel()
        {
            this.cityDataService = new CityDataService();
            this.weatherDataService = new WeatherDataService();
            this.currentWeatherData = weatherDataService.GetEmptyHourlyData();

            GetAndSetConfiguration();

            this.ResourceFounded = true;
            this.IsReady = false;
            this.HasStarted = false;
            this.IsConnected = true;
            this.IsForecastPanelVisible = false;
            this.IsOptionsPanelVisible = false;
            this.ShowMapCommand = new RelayCommand(ShowMap);
            this.OpenSiteCommand = new RelayCommand(OpenWeathermapSite);
            this.RefreshCommand = new RelayCommand(Refresh);
            this.SearchCommand = new RelayCommand(Search, (o) => !string.IsNullOrWhiteSpace((string)o));
            this.ShowForecastCommand = new RelayCommand(ShowForecast, (o) => o != null && (int)o != -1);
            this.ReturnToMainPanelCommand = new RelayCommand(ReturnToMainPanel);
            this.QueryCommand = new RelayCommand((o) => ExecuteQuery(o));
            this.StartingLocationCommand = new RelayCommand((o) => ExecuteQuery(o, true));
            this.StartingLocationResultCommand = new RelayCommand(SetStartingLocation);
            this.QueryResultCommand = new RelayCommand(ShowResult);
            this.ShowOptionsCommand = new RelayCommand(ShowOptions);
            this.SelectLanguageCommand = new RelayCommand(SelectLanguage, (o) => HasStarted);
            this.SelectUnitsCommand = new RelayCommand(SelectUnits, (o) => HasStarted);
            this.RefreshTimeCommand = new RelayCommand(ModifyRefreshTime);
            this.CloseCommand = new RelayCommand(SaveConfiguration);

            Refresh();
        }

        #endregion

        #region Public methods

        public void Refresh()
        {
            GetAndUpdateWeatherData((!HasStarted || CityId == null) ? App.AppSettings.CityId : CityId, true, CallType.ByCityID);
        }

        #endregion

        #region Private methods

        private void SaveConfiguration()
        {
            if (updateConfiguration)
            {
                App.UpdateAndSaveConfiguration();
            }
        }

        private void SelectLanguage(object obj)
        {
            ApplicationLanguage = (Language)(int)obj;
        }

        private void SelectUnits(object obj)
        {
            ApplicationUnits = (Units)(int)obj;
        }

        private void ModifyRefreshTime(object obj)
        {
            RefreshTime = (RefreshTime)(((int)obj + 1) * 15);
        }

        private void OnRefreshTimeChanged(RefreshTime refreshTime)
        {
            App.AppSettings.Interval = (int)refreshTime;

            UpdateAutoUpdateEvent();
            SetUpdateConfigurationFlag();
        }

        private void OnApplicationLanguageChanged(Language language)
        {
            App.AppSettings.Language = language;

            ChangeLanguage();
            SetUpdateConfigurationFlag();
        }

        private void OnApplicationUnitsChanged(Units units)
        {
            App.AppSettings.Units = units;

            ChangeUnits();
            SetUpdateConfigurationFlag();
        }

        private void OnStartinglocationChanged(City startingLocation)
        {
            App.AppSettings.CityId = startingLocation.Id.ToString();

            if (HasStarted)
            {
                ShowResult(startingLocation);
            }

            OnPropertyChanged("StartingLocation");
            SetUpdateConfigurationFlag();
        }

        private void ChangeUnits()
        {
            ViewModelUtils.GetXmlResource(ApplicationUnits).Apply();
            currentWeatherData?.ChangeUnits(ApplicationUnits);
            weatherForecastData?.ChangeUnits(ApplicationUnits);

            UpdateProperties();
        }

        private void ChangeLanguage()
        {
            ViewModelUtils.GetXmlResource(ApplicationLanguage).Apply();
            currentWeatherData?.ChangeLanguage(ApplicationLanguage);
            weatherForecastData?.ChangeLanguage(ApplicationLanguage);

            UpdateProperties();
        }

        private void Search(object obj)
        {
            GetAndUpdateWeatherData(obj, true, CallType.ByCityName);
            refreshCallTypeMethod = CallType.ByCityName;
        }

        private void AutoRefresh()
        {
            GetAndUpdateWeatherData(refreshCallTypeMethod == CallType.ByCityName ? CityName : CityId, false, refreshCallTypeMethod);
        }

        private void ShowResult(object obj)
        {
            GetAndUpdateWeatherData(((City)obj).Id, true, CallType.ByCityID);
            refreshCallTypeMethod = CallType.ByCityID;
        }

        private void SetStartingLocation(object obj)
        {
            StartingLocation = (City)obj;
        }

        private void GetAndUpdateWeatherData(object obj, bool isRefreshIndicatorVisible, CallType callType)
        {
            var arg = new GetWeatherDataEventTransferObject()
            {
                CallType = callType,
                IsRefreshIndicatorVisible = isRefreshIndicatorVisible,
                Argument = obj.ToString()
            };

            Task.Run(() => TryGetCurrentWeatherData(arg))
                .ContinueWith(t => UpdateWeatherData(t.Result), TaskScheduler.FromCurrentSynchronizationContext());

            AutoUpdateEvent.Restart();
        }

        private void ExecuteQuery(object obj, bool isStartingLocationQuery = false)
        {
            searchText = obj.ToString();

            Task.Run(() => new QueryExecutionTransferObject()
            {
                Argument = obj.ToString(),
                IsStartingLocationQuery = isStartingLocationQuery,
                Result = cityDataService.GetCityListByName(obj.ToString())
            }).ContinueWith(t => VerifyQueryResult(t.Result), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void VerifyQueryResult(QueryExecutionTransferObject queryExecutionTransferObject)
        {
            if (searchText == queryExecutionTransferObject.Argument)
            {
                QueryResult = queryExecutionTransferObject;
            }
        }

        private void ReturnToMainPanel(object obj)
        {
            this.IsForecastPanelVisible = false;
            this.ResourceFounded = true;
        }

        private void ShowOptions(object obj)
        {
            this.IsOptionsPanelVisible = !this.IsOptionsPanelVisible;
        }

        private void ShowForecast(object obj)
        {
            DailyForecast = ViewModelUtils.GetHourlyForecastForSpecificDate(weatherForecastData.HourlyData, ApplicationLanguage, DateTime.Now.AddDays((int)obj + 1)).ToList();
            OnPropertyChanged("DailyForecast");
            this.IsForecastPanelVisible = true;
        }

        private void ShowMap()
        {
            Process.Start($"https://www.google.com/maps/place/{Latitude}+{Longitude}");
        }

        private void OpenWeathermapSite()
        {
            Process.Start($"https://openweathermap.org/");
        }

        private void UpdateAutoUpdateEvent()
        {
            AutoUpdateEvent.UpdateInterval(RefreshTime);
        }

        private void UpdateWeatherData(bool isGetAndUpdateWeatherDataSucceed)
        {
            SetHasStartedFlag();

            if (!isGetAndUpdateWeatherDataSucceed)
            {
                IsReady = true;
                return;
            }

            ChangeUnits();
            ChangeLanguage();
            IsReady = true;
        }

        private bool TryGetCurrentWeatherData(GetWeatherDataEventTransferObject arg)
        {
            try
            {
                string oldIcon = currentWeatherData.Icon;

                IsReady = !arg.IsRefreshIndicatorVisible;
                System.Threading.Thread.Sleep(HasStarted ? 500 : 2000);
                weatherForecastData = weatherDataService.GetFourDaysData(arg.CallType, arg.Argument);
                currentWeatherData = weatherDataService.GetHourlyData(arg.CallType, arg.Argument);

                if (!arg.IsRefreshIndicatorVisible && oldIcon != currentWeatherData.Icon)
                {
                    IsReady = false;
                }

                HourlyForecast = ViewModelUtils.GetHourlyForecastFromHourlyData(weatherForecastData.HourlyData);
                FourDaysForecast = weatherForecastData.DailyData.ToList();
                IsConnected = true;
            }
            catch (HttpOperationException)
            {
                ResourceFounded = false;
                return false;
            }
            catch (TaskCanceledException)
            {
                IsConnected = false;
                return false;
            }

            return true;
        }

        private void UpdateProperties()
        {
            var propertiesToUpdate = new List<string>()
                {
                    "Humidity",
                    "Pressure",
                    "CityName",
                    "Description",
                    "UpdateTime",
                    "SunsetTime",
                    "SunriseTime",
                    "Country",
                    "Longitude",
                    "Latitude",
                    "WindAngle",
                    "SnowFall",
                    "RainFall",
                    "Icon",
                    "BackgroundImage",
                    "DescriptionId",
                    "AvgTemp",
                    "FeelTemp",
                    "WindSpeed",
                    "FourDaysForecast",
                    "HourlyForecast",
                    "DailyForecast"
                };

            propertiesToUpdate.ForEach(property => OnPropertyChanged(property));
        }

        private void SetUpdateConfigurationFlag()
        {
            if (!updateConfiguration)
            {
                updateConfiguration = true;
            }
        }

        private void GetAndSetConfiguration()
        {
            ApplicationLanguage = App.AppSettings.Language;
            ApplicationUnits = App.AppSettings.Units;
            RefreshTime = (RefreshTime)App.AppSettings.Interval;
            StartingLocation = cityDataService.GetCityById(App.AppSettings.CityId);
        }

        private void SetHasStartedFlag()
        {
            if (!HasStarted)
            {
                HasStarted = true;
            }
        }

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
