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
using WebApi.Models.Interfaces;
using WebApi.Models.Factories;
using Microsoft.Rest;
using System.Collections.ObjectModel;
using WeatherBar.Models;
using WebApi.Models.Enums;

namespace WeatherBar.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly Timer autoUpdateTimer = new Timer();

        private IHourlyData currentWeatherData = WeatherDataFactory.GetHourlyDataTransferObject();

        private IFourDaysData weatherForecastData;

        private bool isReady;

        private bool isConnected;

        private bool hasStarted;

        private bool resourceFounded;

        private bool isForecastPanelVisible;

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

        public string CityName => currentWeatherData.CityName;

        public string Description => currentWeatherData.Description;

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
                var imageData = AppResources.Utils.GetImageWithHexColor(Icon, FeelTemp, Description);

                return new KeyValuePair<BitmapImage, Color>(
                    Utils.LoadImage(imageData.Key), (Color)ColorConverter.ConvertFromString(imageData.Value));
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

        public MainViewModel()
        {
            currentWeatherData.CityName = App.WebApiClient.CityName;
            this.ResourceFounded = true;
            this.IsReady = false;
            this.HasStarted = false;
            this.IsConnected = true;
            this.IsForecastPanelVisible = false;
            this.ShowMapCommand = new RelayCommand((o) => ShowMap());
            this.OpenSiteCommand = new RelayCommand((o) => OpenWeathermapSite());
            this.RefreshCommand = new RelayCommand((o) => Refresh(CityName, true));
            this.SearchCommand = new RelayCommand((o) => Refresh(o, true), (o) => !string.IsNullOrWhiteSpace((string)o));
            this.ShowForecastCommand = new RelayCommand(ShowForecast, (o) => o != null && (int)o != -1);
            this.ReturnToMainPanelCommand = new RelayCommand(ReturnToMainPanel);
            this.QueryCommand = new RelayCommand(ExecuteQuery);
            this.ResultCommand = new RelayCommand(Refresh);

            Refresh(CityName, true);
            StartAutoUpdateEvent();
        }

        #endregion

        #region Public methods

        public void Refresh(object obj, bool isRefreshIndicatorVisible)
        {
            RefreshBase(obj, isRefreshIndicatorVisible, CallType.ByCityName);
        }

        #endregion

        #region Private methods

        public void Refresh(object sender, ElapsedEventArgs e)
        {
            RefreshBase(CityName, false, CallType.ByCityName);
        }

        public void Refresh(object obj)
        {
            RefreshBase(obj, true, CallType.ByCityID);
        }

        private void RefreshBase(object obj, bool isRefreshIndicatorVisible, CallType callType)
        {
            using (var refreshWorker = new BackgroundWorker())
            {
                var arg = new DoWorkEventArgs(callType);

                IsReady = !isRefreshIndicatorVisible;
                refreshWorker.DoWork += (s, e) => GetCurrentWeather(obj, arg);
                refreshWorker.RunWorkerCompleted += UpdateWeatherData;
                refreshWorker.RunWorkerAsync();
            }

            autoUpdateTimer.Stop();
            autoUpdateTimer.Start();
        }

        private void ExecuteQuery(object obj)
        {
            using (var queryBackgroundWorker = new BackgroundWorker())
            {
                ObservableCollection<City> result = null;

                queryBackgroundWorker.DoWork += (s, e) =>
                {
                    result = new ObservableCollection<City>(Utils.GetCityList((string)obj));
                };
                queryBackgroundWorker.RunWorkerCompleted += (s, e) => QueryResult = result;
                queryBackgroundWorker.RunWorkerAsync();
            }
        }

        private void ReturnToMainPanel(object obj)
        {
            this.IsForecastPanelVisible = false;
            this.ResourceFounded = true;
        }

        private void ShowForecast(object obj)
        {
            DailyForecast = Utils.GetHourlyForecastForSpecificDate(weatherForecastData.HourlyData, DateTime.Now.AddDays((int)obj + 1).ToString("dd MMMM")).ToList();
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
            autoUpdateTimer.Interval = (int)TimeSpan.FromMinutes(App.WebApiClient.Interval).TotalMilliseconds;
            autoUpdateTimer.Elapsed += Refresh;
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

            UpdateProperties();
            IsReady = true;
        }

        private void GetCurrentWeather(object input, DoWorkEventArgs e)
        {
            try
            {
                System.Threading.Thread.Sleep(HasStarted ? 500 : 2000);

                if ((CallType)e.Argument == CallType.ByCityName)
                {
                    weatherForecastData = App.WebApiClient.GetFourDaysForecastDataByCityName((string)input);
                    currentWeatherData = App.WebApiClient.GetCurrentWeatherDataByCityName((string)input);
                }
                else
                {
                    weatherForecastData = App.WebApiClient.GetFourDaysForecastDataByCityId(((City)input).Id.ToString());
                    currentWeatherData = App.WebApiClient.GetCurrentWeatherDataByCityId(((City)input).Id.ToString());
                }

                HourlyForecast = Utils.GetHourlyForecast(weatherForecastData.HourlyData);
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
                    "FourDaysForecast",
                    "HourlyForecast",
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
