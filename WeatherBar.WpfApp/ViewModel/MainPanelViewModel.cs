using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using WeatherBar.WpfApp.ViewModel.Templates;
using WeatherBar.Model;
using System.Linq;
using WeatherBar.Application.Events.Interfaces;
using WeatherBar.Application.Events;
using WeatherBar.Application.Commands;
using WeatherBar.DataProviders.Interfaces;
using WeatherBar.DataProviders;

namespace WeatherBar.WpfApp.ViewModel
{
    public class MainPanelViewModel : ViewModelBase,
        IEventHandler<CurrentWeatherDataUpdatedEvent>,
        IEventHandler<FourDaysForecastDataUpdatedEvent>, 
        IEventHandler<ForecastPanelVisibilityChangedEvent>
    {
        #region Fields

        private readonly ICityDataProvider cityDataProvider;

        private HourlyForecast currentWeatherData = Model.HourlyForecast.Empty();

        private FourDaysForecast fourDaysWeatherData = Model.FourDaysForecast.Empty();

        private bool isForecastPanelVisible;

        private string searchText;

        private QueryExecution queryResult;

        #endregion

        #region Public properties

        public ICommand ShowMapCommand { get; private set; }

        public ICommand SearchCommand { get; private set; }

        public ICommand QueryCommand { get; private set; }

        public ICommand QueryResultCommand { get; private set; }

        public ICommand ShowForecastCommand { get; private set; }

        public bool IsForecastPanelVisible
        {
            get => isForecastPanelVisible;
            set
            {
                isForecastPanelVisible = value;
                Notify(new ForecastPanelVisibilityChangedEvent(this, value));
            }
        }

        public string CityName => currentWeatherData.CityName;

        public string CityId => currentWeatherData.CityId;

        public string Description => currentWeatherData.Description;

        public string DescriptionId => currentWeatherData.DescriptionId;

        public int AvgTemp => currentWeatherData.AvgTemp;

        public int FeelTemp => currentWeatherData.FeelTemp;

        public double SnowFall => currentWeatherData.SnowFall;

        public double RainFall => currentWeatherData.RainFall;

        public DateTime UpdateTime => currentWeatherData.UpdateTime;

        public string SunsetTime => currentWeatherData.SunsetTime;

        public string SunriseTime => currentWeatherData.SunriseTime;

        public string Country => currentWeatherData.Country;

        public double Longtitude => currentWeatherData.Longtitude;

        public double Latitude => currentWeatherData.Latitude;

        public int Pressure => currentWeatherData.Pressure;

        public int Humidity => currentWeatherData.Humidity;

        public int WindSpeed => currentWeatherData.WindSpeed;

        public int WindAngle => currentWeatherData.WindAngle;

        public string Icon => currentWeatherData.Icon;

        public List<DailyForecast> FourDaysForecast => fourDaysWeatherData.DailyData;

        public Tuple<IEnumerable<HourlyForecast>, IEnumerable<HourlyForecast>> HourlyForecast => GetHourlyForecastFromHourlyData(fourDaysWeatherData.HourlyData);

        public QueryExecution QueryResult
        {
            get => queryResult;
            private set
            {
                queryResult = value;
                Notify(new QueryResultUpdatedEvent(this, value));
            }
        }

        #endregion

        #region Constructors

        public MainPanelViewModel()
        {
            this.cityDataProvider = new CityDataProvider();
            this.ShowMapCommand = new RelayCommand(ShowMap);
            this.SearchCommand = new RelayCommand(Search, (o) => !string.IsNullOrWhiteSpace((string)o));
            this.ShowForecastCommand = new RelayCommand(ShowForecast, (o) => o != null && (int)o != -1);
            this.QueryCommand = new RelayCommand((o) => ExecuteQuery(o));
            this.QueryResultCommand = new RelayCommand(ShowResult);
        }

        #endregion

        #region Public methods

        public void Handle(CurrentWeatherDataUpdatedEvent @event)
        {
            currentWeatherData = @event.Content;
        }

        public void Handle(ForecastPanelVisibilityChangedEvent @event)
        {
            IsForecastPanelVisible = @event.Content;
        }

        public void Handle(FourDaysForecastDataUpdatedEvent @event)
        {
            fourDaysWeatherData = @event.Content;
        }

        public void Handle(StartingLocationUpdatedEvent @event)
        {
            ShowResult(@event.Content);
        }

        #endregion

        #region Private methods

        private void Search(object obj)
        {
            Notify(new WeatherDataUpdatedEvent(this, obj.ToString()));
        }

        private void ShowResult(object obj)
        {
            Notify(new WeatherDataUpdatedEvent(this, ((City)obj).Id.ToString()));
        }

        private void ExecuteQuery(object obj)
        {
            searchText = obj.ToString();

            Task.Run(() => new QueryExecution()
            {
                Argument = obj.ToString(),
                Result = cityDataProvider.GetCityListByName(obj.ToString())
            }).ContinueWith(t => VerifyQueryResult(t.Result), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void VerifyQueryResult(QueryExecution queryExecution)
        {
            if (searchText == queryExecution.Argument)
            {
                QueryResult = queryExecution;
            }
        }

        private void ShowForecast(object obj)
        {
            Notify(new ShowDailyForecastEvent(this, DateTime.Now.AddDays((int)obj + 1)));

            this.IsForecastPanelVisible = true;
        }

        private void ShowMap()
        {
            Process.Start($"https://www.google.com/maps/place/{Latitude}+{Longtitude}");
        }

        private Tuple<IEnumerable<HourlyForecast>, IEnumerable<HourlyForecast>> GetHourlyForecastFromHourlyData(IEnumerable<HourlyForecast> hourlyData)
        {
            return new Tuple<IEnumerable<HourlyForecast>, IEnumerable<HourlyForecast>>(hourlyData.Take(5), hourlyData.Skip(5).Take(5));
        }

        #endregion
    }
}
