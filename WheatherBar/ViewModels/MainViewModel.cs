using System;
using System.Timers;
using System.ComponentModel;
using System.Windows.Input;
using WeatherBar.WebApi.Models;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;

namespace WeatherBar.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly Timer autoUpdateTimer = new Timer();

        private CurrentWeatherData currentWeatherData;

        private bool isReady;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public ICommand ShowMapCommand { get; private set; }

        public ICommand OpenSiteCommand { get; private set; }

        public ICommand RefreshCommand { get; private set; }

        public ICommand SearchCommand { get; private set; }

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

        public string CityName
        {
            get
            {
                return currentWeatherData?.Name;
            }
        }

        public string Description
        {
            get
            {
                if (currentWeatherData != null)
                {
                    var input = currentWeatherData.WheatherData[0].Description;
                    return input.FirstOrDefault().ToString().ToUpper() + input.Substring(1);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public int AvgTemp
        {
            get
            {
                return Convert.ToInt32(currentWeatherData?.MainData.Temp);
            }
        }

        public int FeelTemp
        {
            get
            {
                return Convert.ToInt32(currentWeatherData?.MainData.FeelsLike);
            }
        }

        public double SnowFall
        {
            get
            {
                return Convert.ToDouble(currentWeatherData?.SnowData._1h);
            }
        }

        public double RainFall
        {
            get
            {
                return Convert.ToDouble(currentWeatherData?.RainData._1h);
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
                if (currentWeatherData != null)
                {
                    return (SharedFunctions.UnixTimeStampToDateTime(currentWeatherData.SysData.Sunset) + DateTimeOffset.Now.Offset).ToString("HH:mm");
                }

                return string.Empty;
            }
        }

        public string SunriseTime
        {
            get
            {
                if (currentWeatherData != null)
                {
                    return (SharedFunctions.UnixTimeStampToDateTime(currentWeatherData.SysData.Sunrise) + DateTimeOffset.Now.Offset).ToString("HH:mm");
                }

                return string.Empty;
            }
        }

        public string Country
        {
            get
            {
                return currentWeatherData?.SysData.Country;
            }
        }

        public string Longitude
        {
            get
            {
                if (currentWeatherData != null)
                {
                    return ConvertDecToDeg(currentWeatherData.CoordData.Lon, true);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string Latitude
        {
            get
            {
                if (currentWeatherData != null)
                {
                    return ConvertDecToDeg(currentWeatherData.CoordData.Lat, false);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public int Pressure
        {
            get
            {
                return Convert.ToInt32(currentWeatherData?.MainData.Pressure);
            }
        }

        public int Humidity
        {
            get
            {
                return Convert.ToInt32(currentWeatherData?.MainData.Humidity);
            }
        }

        public int WindSpeed
        {
            get
            {
                return Convert.ToInt32(currentWeatherData?.WindData.Speed * 3.6);
            }
        }

        public int WindAngle
        {
            get
            {
                return Convert.ToInt32(currentWeatherData?.WindData.Deg - 180);
            }
        }

        public string Icon
        {
            get
            {
                if (currentWeatherData != null)
                {
                    return currentWeatherData.WheatherData[0].Icon;
                }
                else
                {
                    return "01d";
                }
            }
        }

        public KeyValuePair<BitmapImage, Color> BackgroundImage
        {
            get
            {
                var imageData = AppData.DataContainer.GetImageWithHexColor(Icon, FeelTemp);

                return new KeyValuePair<BitmapImage, Color>(
                    SharedFunctions.LoadImage(imageData.Key), (Color)ColorConverter.ConvertFromString(imageData.Value));
            }
        }

        #endregion

        #region Constructors

        public MainViewModel()
        {
            this.IsReady = false;
            this.ShowMapCommand = new RelayCommand((o) => ShowMap());
            this.OpenSiteCommand = new RelayCommand((o) => OpenSite());
            this.RefreshCommand = new RelayCommand(Refresh);
            this.SearchCommand = new RelayCommand(Refresh);

            Refresh();
            StartAutoUpdateEvent();
        }

        #endregion

        #region Methods

        public void Refresh(object obj = null)
        {
            obj = obj ?? (!string.IsNullOrEmpty(CityName) ? CityName : "Warszawa");

            using (var refreshWorker = new BackgroundWorker())
            {
                refreshWorker.DoWork += (s, e) => LoadCurrentWeather(obj.ToString(), e);
                refreshWorker.RunWorkerCompleted += UpdateWeatherData;
                refreshWorker.RunWorkerAsync();
            }

            autoUpdateTimer.Stop();
            autoUpdateTimer.Start();
        }

        public void Refresh(object sender, ElapsedEventArgs e)
        {
            Refresh(CityName);
        }

        #endregion

        #region Private methods

        private void ShowMap()
        {
            System.Diagnostics.Process.Start($"https://www.google.com/maps/place/{Latitude}+{Longitude}");
        }

        private void OpenSite()
        {
            System.Diagnostics.Process.Start($"https://openweathermap.org/");
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
            if (e.Cancelled)
            {
                IsReady = false;
                return;
            }

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
                };

            propertiesToUpdate.ForEach(property => OnPropertyChanged(property));
            IsReady = true;
        }

        private string ConvertDecToDeg(double decValue, bool isLongitude)
        {
            string direction;

            if (decValue > 0)
            {
                if (isLongitude)
                {
                    direction = "E";
                }
                else
                {
                    direction = "N";
                }
            }
            else
            {
                if (isLongitude)
                {
                    direction = "W";
                }
                else
                {
                    direction = "S";
                }
            }

            var temp = Math.Round(decValue > 0 ? decValue : -decValue, 2).ToString().Split('.', ',');
            var minutesValue = Math.Round(double.Parse(temp.Last()) * 60 / 100).ToString();

            return string.Concat(temp.First(), "° ", minutesValue.Length != 1 ? minutesValue : $"0{minutesValue}", $"' {direction}");
        }

        private void LoadCurrentWeather(string cityName, DoWorkEventArgs e)
        {
            try
            {
                currentWeatherData = App.WebApiClient.GetCurrentWeatherData(cityName);
            }
            catch (TaskCanceledException)
            {
                e.Cancel = true;
            }
        }

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
