using System;
using System.Timers;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using WeatherBar.Utils;
using System.Diagnostics;
using WeatherBar.WebApi.Models.Interfaces;
using WeatherBar.WebApi.Models.Factories;
using Microsoft.Rest;

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

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public ICommand ShowMapCommand { get; private set; }

        public ICommand OpenSiteCommand { get; private set; }

        public ICommand RefreshCommand { get; private set; }

        public ICommand SearchCommand { get; private set; }

        public ICommand ShowForecastCommand { get; private set; }

        public ICommand ReturnToMainPanelCommand { get; private set; }

        public bool IsReady
        {
            get
            {
                return isReady;
            }
            private set
            {
                isReady = value;
                OnPropertyChanged();
            }
        }

        public bool HasStarted
        {
            get
            {
                return hasStarted;
            }
            private set
            {
                hasStarted = value;
                OnPropertyChanged();
            }
        }

        public bool ResourceFounded
        {
            get
            {
                return resourceFounded;
            }
            private set
            {
                resourceFounded = value;
                OnPropertyChanged();
            }
        }

        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
            private set
            {
                isConnected = value;
                OnPropertyChanged();
            }
        }

        public bool IsForecastPanelVisible
        {
            get
            {
                return isForecastPanelVisible;
            }
            set
            {
                isForecastPanelVisible = value;
                OnPropertyChanged();
            }
        }

        public string CityName
        {
            get
            {
                return currentWeatherData.CityName;
            }
        }

        public string Description
        {
            get
            {
                return currentWeatherData.Description;
            }
        }

        public int AvgTemp
        {
            get
            {
                return currentWeatherData.AvgTemp;
            }
        }

        public int FeelTemp
        {
            get
            {
                return currentWeatherData.FeelTemp;
            }
        }

        public double SnowFall
        {
            get
            {
                return currentWeatherData.SnowFall;
            }
        }

        public double RainFall
        {
            get
            {
                return currentWeatherData.RainFall;
            }
        }

        public string UpdateTime
        {
            get
            {
                return DateTime.Now.ToString("HH:mm");
            }
        }

        public string SunsetTime
        {
            get
            {
                return currentWeatherData.SunsetTime;
            }
        }

        public string SunriseTime
        {
            get
            {
                return currentWeatherData.SunriseTime;
            }
        }

        public string Country
        {
            get
            {
                return currentWeatherData.Country;
            }
        }

        public string Longitude
        {
            get
            {
                return currentWeatherData.Longitude;
            }
        }

        public string Latitude
        {
            get
            {
                return currentWeatherData.Latitude;
            }
        }

        public int Pressure
        {
            get
            {
                return currentWeatherData.Pressure;
            }
        }

        public int Humidity
        {
            get
            {
                return currentWeatherData.Humidity;
            }
        }

        public int WindSpeed
        {
            get
            {
                return currentWeatherData.WindSpeed;
            }
        }

        public int WindAngle
        {
            get
            {
                return currentWeatherData.WindAngle;
            }
        }

        public string Icon
        {
            get
            {
                return currentWeatherData.Icon;
            }
        }

        public KeyValuePair<BitmapImage, Color> BackgroundImage
        {
            get
            {
                var imageData = AppData.DataContainer.GetImageWithHexColor(Icon, FeelTemp, Description);

                return new KeyValuePair<BitmapImage, Color>(
                    SharedFunctions.LoadImage(imageData.Key), (Color)ColorConverter.ConvertFromString(imageData.Value));
            }
        }

        public List<IDailyData> FourDaysForecast { get; private set; }

        public Tuple<List<IHourlyData>, List<IHourlyData>> HourlyForecast { get; private set; }

        public List<IHourlyData> DailyForecast { get; private set; }

        #endregion

        #region Constructors

        public MainViewModel()
        {
            this.ResourceFounded = true;
            this.IsReady = false;
            this.HasStarted = false;
            this.IsConnected = true;
            this.IsForecastPanelVisible = false;
            this.ShowMapCommand = new RelayCommand((o) => ShowMap());
            this.OpenSiteCommand = new RelayCommand((o) => OpenWeathermapSite());
            this.RefreshCommand = new RelayCommand((o) => Refresh(CityName));
            this.SearchCommand = new RelayCommand((o) => Refresh(o), (o) => !string.IsNullOrWhiteSpace((string)o));
            this.ShowForecastCommand = new RelayCommand(ShowForecast, (o) => o != null && (int)o != -1);
            this.ReturnToMainPanelCommand = new RelayCommand(ReturnToMainPanel);

            Refresh(CityName);
            StartAutoUpdateEvent();
        }

        #endregion

        #region Public methods

        public void Refresh(object obj = null, bool isRefreshIndicatorVisible = true)
        {
            using (var refreshWorker = new BackgroundWorker())
            {
                IsReady = !isRefreshIndicatorVisible;
                refreshWorker.DoWork += (s, e) => LoadCurrentWeather((string)obj, e);
                refreshWorker.RunWorkerCompleted += UpdateWeatherData;
                refreshWorker.RunWorkerAsync();
            }

            autoUpdateTimer.Stop();
            autoUpdateTimer.Start();
        }

        public void Refresh(object sender, ElapsedEventArgs e)
        {
            Refresh(CityName, false);
        }

        #endregion

        #region Private methods

        private void ReturnToMainPanel(object obj)
        {
            this.IsForecastPanelVisible = false;
            this.ResourceFounded = true;
        }

        private void ShowForecast(object obj)
        {
            DailyForecast = SharedFunctions.GetHourlyForecastForSpecificDate(weatherForecastData.HourlyData, DateTime.Now.AddDays((int)obj + 1).ToString("dd MMMM")).ToList();
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
            autoUpdateTimer.Interval = (int)TimeSpan.FromMinutes(15).TotalMilliseconds;
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

        private void LoadCurrentWeather(string cityName, DoWorkEventArgs e)
        {
            try
            {
                System.Threading.Thread.Sleep(HasStarted ? 500 : 2000);

                weatherForecastData = App.WebApiClient.GetFourDaysForecastData(cityName);
                currentWeatherData = App.WebApiClient.GetCurrentWeatherData(cityName);
                HourlyForecast = SharedFunctions.GetHourlyForecast(weatherForecastData.HourlyData);
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
