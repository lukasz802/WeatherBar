﻿using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using WeatherBar.Model.DataTransferObjects;
using WeatherBar.Model.Interfaces;
using WeatherBar.Model.Services;
using WeatherBar.Model.Services.Interfaces;
using WeatherBar.ViewModel.Templates;
using WeatherBar.Model;
using WeatherBar.Core.Events;
using WeatherBar.Core.Commands;

namespace WeatherBar.ViewModel
{
    public class MainPanelViewModel : ViewModelBase
    {
        #region Fields

        private readonly ICityDataService cityDataService;

        private IHourlyData currentWeatherData;

        private bool isConnected;

        private bool isForecastPanelVisible;

        private bool isReady;

        private string searchText;

        private QueryExecutionTransferObject queryResult;

        #endregion

        #region Public properties

        public ICommand ShowMapCommand { get; private set; }

        public ICommand SearchCommand { get; private set; }

        public ICommand QueryCommand { get; private set; }

        public ICommand QueryResultCommand { get; private set; }

        public ICommand ShowForecastCommand { get; private set; }

        public bool IsConnected
        {
            get => isConnected;
            private set
            {
                isConnected = value;
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

        public bool IsReady
        {
            get => isReady;
            private set
            {
                isReady = value;
                Notify();
            }
        }

        public string CityName => CurrentWeatherData.CityName;

        public string CityId => CurrentWeatherData.CityId?.ToString();

        public string Description => CurrentWeatherData.Description;

        public string DescriptionId => CurrentWeatherData.DescriptionId;

        public int AvgTemp => CurrentWeatherData.AvgTemp;

        public int FeelTemp => CurrentWeatherData.FeelTemp;

        public double SnowFall => CurrentWeatherData.SnowFall;

        public double RainFall => CurrentWeatherData.RainFall;

        public string UpdateTime => DateTime.Now.ToString("HH:mm");

        public string SunsetTime => CurrentWeatherData.SunsetTime;

        public string SunriseTime => CurrentWeatherData.SunriseTime;

        public string Country => CurrentWeatherData.Country;

        public string Longitude => CurrentWeatherData.Longitude;

        public string Latitude => CurrentWeatherData.Latitude;

        public int Pressure => CurrentWeatherData.Pressure;

        public int Humidity => CurrentWeatherData.Humidity;

        public int WindSpeed => CurrentWeatherData.WindSpeed;

        public int WindAngle => CurrentWeatherData.WindAngle;

        public string Icon => CurrentWeatherData.Icon;

        public List<IDailyData> FourDaysForecast { get; private set; }

        public Tuple<List<IHourlyData>, List<IHourlyData>> HourlyForecast { get; private set; }

        public QueryExecutionTransferObject QueryResult
        {
            get => queryResult;
            private set
            {
                queryResult = value;
                Notify();
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
            set => currentWeatherData = value;
        }

        #endregion

        #region Constructors

        public MainPanelViewModel()
        {
            this.AutomaticallyApplyReceivedChanges = true;
            this.IncludeOnlyPublicChanges = false;
            this.MessageReceived += MainPanelViewModel_MessageReceived;
            this.cityDataService = new CityDataService();
            this.ShowMapCommand = new RelayCommand(ShowMap);
            this.SearchCommand = new RelayCommand(Search, (o) => !string.IsNullOrWhiteSpace((string)o));
            this.ShowForecastCommand = new RelayCommand(ShowForecast, (o) => o != null && (int)o != -1);
            this.QueryCommand = new RelayCommand((o) => ExecuteQuery(o));
            this.QueryResultCommand = new RelayCommand(ShowResult);
        }

        #endregion

        #region Private methods

        private void MainPanelViewModel_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.CallerName == "StartingLocationUpdated")
            {
                ShowResult(e.Message);
            }

            if (e.CallerName == "WeatherDataUpdated" || e.CallerName == "UnitsUpdated" ||
                e.CallerName == "LanguageUpdated" || e.CallerName == "StartingLocationUpdated")
            {
                UpdateProperties();
            }
        }

        private void Search(object obj)
        {
            Notify("UpdateWeatherData", new GetWeatherDataEventTransferObject()
            {
                Argument = obj.ToString(),
                IsRefreshIndicatorVisible = true
            });
        }

        private void ShowResult(object obj)
        {
            Notify("UpdateWeatherData", new GetWeatherDataEventTransferObject()
            {
                Argument = ((City)obj).Id.ToString(),
                IsRefreshIndicatorVisible = true
            });
        }

        private void ExecuteQuery(object obj)
        {
            searchText = obj.ToString();

            Task.Run(() => new QueryExecutionTransferObject()
            {
                Argument = obj.ToString(),
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

        private void ShowForecast(object obj)
        {
            Notify("ShowDailyForecast", DateTime.Now.AddDays((int)obj + 1));
            this.IsForecastPanelVisible = true;
        }

        private void ShowMap()
        {
            Process.Start($"https://www.google.com/maps/place/{Latitude}+{Longitude}");
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
                    "DescriptionId",
                    "AvgTemp",
                    "FeelTemp",
                    "WindSpeed",
                    "FourDaysForecast",
                    "HourlyForecast",
                    "DailyForecast"
                };

            propertiesToUpdate.ForEach(property => Notify(property));
        }

        #endregion
    }
}