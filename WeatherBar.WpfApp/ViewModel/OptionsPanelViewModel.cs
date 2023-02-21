using System.Threading.Tasks;
using System.Windows.Input;
using WeatherBar.Application.Commands;
using WeatherBar.Application.Events;
using WeatherBar.Application.Events.Interfaces;
using WeatherBar.DataProviders;
using WeatherBar.DataProviders.Interfaces;
using WeatherBar.Model;
using WeatherBar.Model.Enums;
using WeatherBar.WpfApp.ViewModel.Templates;

namespace WeatherBar.WpfApp.ViewModel
{
    public class OptionsPanelViewModel : ViewModelBase, IEventHandler<AppStatusChangedEvent>, IEventHandler<StartingLocationUpdatedEvent>
    {
        #region Fields

        private readonly ICityDataProvider cityDataProvider;

        private RefreshTime refreshTime;

        private Units appUnits;

        private Language appLanguage;

        private City startingLocation;

        private bool hasStarted;

        private string searchText;

        private QueryExecution startingLocationQueryResult;

        #endregion

        #region Public properties

        public ICommand SelectUnitsCommand { get; private set; }

        public ICommand SelectLanguageCommand { get; private set; }

        public ICommand RefreshTimeCommand { get; private set; }

        public ICommand StartingLocationCommand { get; private set; }

        public ICommand StartingLocationResultCommand { get; private set; }

        public RefreshTime RefreshTime
        {
            get => refreshTime;
            set
            {
                if (value != refreshTime)
                {
                    refreshTime = value;
                    Notify(new RefreshTimeUpdatedEvent(this, value));
                }
            }
        }

        public Units ApplicationUnits
        {
            get => appUnits;
            set
            {
                if (value != appUnits)
                {
                    appUnits = value;
                    Notify(new UnitsUpdatedEvent(this, value));
                }
            }
        }

        public Language ApplicationLanguage
        {
            get => appLanguage;
            set
            {
                if (value != appLanguage)
                {
                    appLanguage = value;
                    Notify(new LanguageUpdatedEvent(this, value));
                }
            }
        }

        public City StartingLocation
        {
            get => startingLocation;
            private set
            {
                if (startingLocation == null)
                {
                    startingLocation = value;
                    return;
                }

                startingLocation = value;

                Notify(new StartingLocationUpdatedEvent(this, value));
            }
        }

        public QueryExecution StartingLocationQueryResult
        {
            get => startingLocationQueryResult;
            private set
            {
                startingLocationQueryResult = value;
                Notify(new StartingLocationQueryResultUpdated(this));
            }
        }

        #endregion

        #region Constructors

        public OptionsPanelViewModel()
        {
            LoadConfiguration();

            this.cityDataProvider = new CityDataProvider();
            this.RefreshTimeCommand = new RelayCommand(ChangeRefreshTime, (o) => hasStarted);
            this.SelectUnitsCommand = new RelayCommand(ChangeUnits, (o) => hasStarted);
            this.SelectLanguageCommand = new RelayCommand(ChangeLanguage, (o) => hasStarted);
            this.StartingLocationCommand = new RelayCommand(ExecuteQuery);
            this.StartingLocationResultCommand = new RelayCommand(SetStartingLocation);
        }

        #endregion

        #region Public methods

        public void Handle(AppStatusChangedEvent @event)
        {
            hasStarted = @event.Content != AppStatus.Starting;
        }

        public void Handle(StartingLocationUpdatedEvent @event)
        {
            StartingLocation = @event.Content;
        }

        #endregion

        #region Private methods

        private void ExecuteQuery(object obj)
        {
            searchText = obj.ToString();

            Task.Run(() =>
            {
                var queryExecution = new QueryExecution()
                {
                    Argument = obj.ToString(),
                    Result = cityDataProvider.GetCityListByName(obj.ToString())
                };

                if (searchText == queryExecution.Argument)
                {
                    StartingLocationQueryResult = queryExecution;
                }
            });
        }

        private void ChangeRefreshTime(object obj)
        {
            RefreshTime = (RefreshTime)(((int)obj + 1) * 15);
        }

        private void ChangeUnits(object obj)
        {
            ApplicationUnits = (Units)obj;
        }

        private void ChangeLanguage(object obj)
        {
            ApplicationLanguage = (Language)obj;
        }

        private void SetStartingLocation(object obj)
        {
            StartingLocation = (City)obj;
        }

        private void LoadConfiguration()
        {
            refreshTime = (RefreshTime)App.AppSettings.Interval;
            appLanguage = App.AppSettings.Language;
            appUnits = App.AppSettings.Units;
        }

        #endregion
    }
}
