using System;
using System.Timers;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using WeatherBar.Core;
using System.Diagnostics;
using WebApi.Model.Interfaces;
using WebApi.Model.Factories;
using Microsoft.Rest;
using System.Collections.ObjectModel;
using WeatherBar.Model;
using WebApi.Model.Enums;
using WeatherBar.Utils;
using WeatherBar.Model.DataTransferObjects;
using AppResources;
using WebApi;

namespace WeatherBar.ViewModel
{
    public class AppViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly Timer autoUpdateTimer = new Timer();

        private readonly IWeatherApiCommands apiCommands;

        private IHourlyData currentWeatherData = WeatherDataFactory.GetEmptyHourlyDataTransferObject();

        private IFourDaysData weatherForecastData;

        private CallType refreshCallTypeMethod;

        private Language appLanguage;

        private bool isReady;

        private bool isConnected;

        private bool hasStarted;

        private bool resourceFounded;

        private bool isForecastPanelVisible;

        private bool isOptionsPanelVisible;

        private string searchText;

        private ObservableCollection<City> queryResult;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public ICommand ShowMapCommand { get; private set; }

        public ICommand OpenSiteCommand { get; private set; }

        public ICommand RefreshCommand { get; private set; }

        public ICommand SearchCommand { get; private set; }

        public ICommand QueryCommand { get; private set; }

        public ICommand ResultCommand { get; private set; }

        public ICommand ShowForecastCommand { get; private set; }

        public ICommand ReturnToMainPanelCommand { get; private set; }

        public ICommand ShowOptionsCommand { get; private set; }

        public ICommand SelectLanguageCommand { get; private set; }

        public Language ApplicationLanguage
        {
            get => appLanguage;
            private set
            {
                appLanguage = value;
                OnApplicationLanguageChanged(value);
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

        public string CityId => currentWeatherData.CityId.ToString();

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

        public Tuple<List<IHourlyData>, List<IHourlyData>> HourlyForecast { get; private set; }

        public List<IHourlyData> DailyForecast { get; private set; }

        public ObservableCollection<City> QueryResult
        {
            get => queryResult;
            private set
            {
                queryResult = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public AppViewModel(IWeatherApiCommands weatherApiCommands)
        {
            this.apiCommands = weatherApiCommands;
            this.ApplicationLanguage = App.Language;
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
            this.QueryCommand = new RelayCommand(ExecuteQuery);
            this.ResultCommand = new RelayCommand(ShowResult);
            this.ShowOptionsCommand = new RelayCommand(ShowOptions);
            this.SelectLanguageCommand = new RelayCommand(SelectLanguage, (o) => HasStarted);

            Refresh();
            StartAutoUpdateEvent();
        }

        #endregion

        #region Public methods

        public void Refresh()
        {
            GetAndUpdateWeatherData(!HasStarted ? App.CityId : CityId, true, CallType.ByCityID);
        }

        #endregion

        #region Private methods

        private void SelectLanguage(object obj)
        {
            ApplicationLanguage = (int)obj == 0 ? Language.Polish : Language.English;
        }

        private void OnApplicationLanguageChanged(Language language)
        {
            App.Language = language;

            ApplicationUtils.TranslateResources(language);
            ChangeLanguage();
            Task.Run(() => App.UpdateAndSaveConfiguration());
        }

        private void ChangeLanguage()
        {
            currentWeatherData?.ChangeLanguage(ApplicationLanguage);
            weatherForecastData?.ChangeLanguage(ApplicationLanguage);

            OnPropertyChanged("Description");
            OnPropertyChanged("FourDaysForecast");
            OnPropertyChanged("HourlyForecast");
            OnPropertyChanged("DailyForecast");
        }

        private void Search(object obj)
        {
            GetAndUpdateWeatherData(obj, true, CallType.ByCityName);
            refreshCallTypeMethod = CallType.ByCityName;
        }

        private void AutoRefresh(object sender, ElapsedEventArgs e)
        {
            GetAndUpdateWeatherData(refreshCallTypeMethod == CallType.ByCityName ? CityName : CityId, false, refreshCallTypeMethod);
        }

        private void ShowResult(object obj)
        {
            GetAndUpdateWeatherData(((City)obj).Id, true, CallType.ByCityID);
            refreshCallTypeMethod = CallType.ByCityID;
        }

        private void GetAndUpdateWeatherData(object obj, bool isRefreshIndicatorVisible, CallType callType)
        {
            using (var refreshWorker = new BackgroundWorker())
            {
                var arg = new GetWeatherDataEventTransferObject()
                {
                    CallType = callType,
                    IsRefreshIndicatorVisible = isRefreshIndicatorVisible,
                    Argument = obj.ToString()
                };

                IsReady = !isRefreshIndicatorVisible;

                refreshWorker.DoWork += GetCurrentWeatherData;
                refreshWorker.RunWorkerCompleted += UpdateWeatherData;
                refreshWorker.RunWorkerAsync(arg);
            }

            autoUpdateTimer.Stop();
            autoUpdateTimer.Start();
        }

        private void ExecuteQuery(object obj)
        {
            searchText = obj.ToString();

            using (var queryBackgroundWorker = new BackgroundWorker())
            {
                queryBackgroundWorker.DoWork += ExecuteQueryDoWork;
                queryBackgroundWorker.RunWorkerCompleted += ExecuteQueryCompleted;
                queryBackgroundWorker.RunWorkerAsync(obj);
            }
        }

        private void ExecuteQueryDoWork(object sender, DoWorkEventArgs e)
        {
            var arg = (string)e.Argument;
            var query = new QueryExecutionTransferObject()
            {
                Argument = arg,
                Result = new ObservableCollection<City>(ViewModelUtils.GetCityList(arg))
            };

            e.Result = query;
        }

        private void ExecuteQueryCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (QueryExecutionTransferObject)e.Result;

            if (searchText == result.Argument)
            {
                QueryResult = result.Result;
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

        private void StartAutoUpdateEvent()
        {
            autoUpdateTimer.AutoReset = true;
            autoUpdateTimer.Interval = (int)TimeSpan.FromMinutes(App.Interval).TotalMilliseconds;
            autoUpdateTimer.Elapsed += AutoRefresh;
            autoUpdateTimer.Start();
        }

        private void UpdateWeatherData(object sender, RunWorkerCompletedEventArgs e)
        {
            SetHasStartedFlag();

            if (e.Cancelled)
            {
                IsReady = true;
                return;
            }

            ChangeLanguage();
            UpdateProperties();
            IsReady = true;
        }

        private void GetCurrentWeatherData(object sender, DoWorkEventArgs e)
        {
            try
            {
                System.Threading.Thread.Sleep(HasStarted ? 500 : 2000);

                string oldIcon = currentWeatherData.Icon;
                var arg = (GetWeatherDataEventTransferObject)e.Argument;

                if (arg.CallType == CallType.ByCityName)
                {
                    weatherForecastData = apiCommands.GetFourDaysForecastDataByCityName(arg.Argument);
                    currentWeatherData = apiCommands.GetCurrentWeatherDataByCityName(arg.Argument);
                }
                else
                {
                    weatherForecastData = apiCommands.GetFourDaysForecastDataByCityId(arg.Argument);
                    currentWeatherData = apiCommands.GetCurrentWeatherDataByCityId(arg.Argument);
                }

                if (!arg.IsRefreshIndicatorVisible && oldIcon != currentWeatherData.Icon)
                {
                    IsReady = false;
                }

                HourlyForecast = ViewModelUtils.GetHourlyForecast(weatherForecastData.HourlyData);
                FourDaysForecast = weatherForecastData.DailyData.ToList();
                IsConnected = true;
            }
            catch (HttpOperationException)
            {
                ResourceFounded = false;
                e.Cancel = true;
            }
            catch (TaskCanceledException)
            {
                IsConnected = false;
                e.Cancel = true;
            }
        }

        private void UpdateProperties()
        {
            var propertiesToUpdate = new List<string>()
                {
                    "CityName",
                    "AvgTemp",
                    "FeelTemp",
                    "Description",
                    "UpdateTime",
                    "SunsetTime",
                    "SunriseTime",
                    "Country",
                    "Longitude",
                    "Latitude",
                    "Pressure",
                    "Humidity",
                    "WindSpeed",
                    "WindAngle",
                    "SnowFall",
                    "RainFall",
                    "Icon",
                    "BackgroundImage",
                    "DescriptionId"
                };

            propertiesToUpdate.ForEach(property => OnPropertyChanged(property));
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
