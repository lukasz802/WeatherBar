using System.Threading.Tasks;
using System.Windows.Input;
using WeatherBar.Core.Commands;
using WeatherBar.Model;
using WeatherBar.Model.DataTransferObjects;
using WeatherBar.Model.Enums;
using WeatherBar.Model.Services;
using WeatherBar.Model.Services.Interfaces;
using WeatherBar.ViewModel.Templates;

namespace WeatherBar.ViewModel
{
    public class OptionsPanelViewModel : ViewModelBase
    {
        #region Fields

        private readonly ICityDataService cityDataService;

        private RefreshTime refreshTime;

        private Units appUnits;

        private Language appLanguage;

        private City startingLocation;

        private bool updateConfiguration;

        private bool hasStarted;

        private string searchText;

        private QueryExecutionTransferObject startingLocationQueryResult;

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
            private set
            {
                if (value != refreshTime)
                {
                    refreshTime = value;
                    OnRefreshTimeChanged();
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
                    OnApplicationUnitsChanged();
                }
            }
        }

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

        public City StartingLocation
        {
            get => startingLocation;
            private set
            {
                if (startingLocation == null)
                {
                    startingLocation = value;
                    Notify();
                    return;
                }

                if (value != startingLocation)
                {
                    startingLocation = value;
                    OnStartinglocationChanged(value);
                    Notify();
                }
            }
        }

        public QueryExecutionTransferObject StartingLocationQueryResult
        {
            get => startingLocationQueryResult;
            private set
            {
                startingLocationQueryResult = value;
                Notify();
            }
        }

        #endregion

        #region Private properties

        private bool UpdateConfiguration 
        {
            get => updateConfiguration;
            set
            {
                updateConfiguration = value;
                Notify();
            }
        }

        private bool HasStarted
        {
            get => hasStarted;
            set
            {
                hasStarted = value;
                Notify();
            }
        }

        #endregion

        #region Constructors

        public OptionsPanelViewModel()
        {
            LoadConfiguration();

            this.AutomaticallyApplyReceivedChanges = true;
            this.MessageReceived += OptionsPanelViewModel_MessageReceived;
            this.UpdateConfiguration = false;
            this.cityDataService = new CityDataService();
            this.RefreshTimeCommand = new RelayCommand(ChangeRefreshTime, (o) => HasStarted);
            this.SelectUnitsCommand = new RelayCommand(ChangeUnits, (o) => HasStarted);
            this.SelectLanguageCommand = new RelayCommand(ChangeLanguage, (o) => HasStarted);
            this.StartingLocationCommand = new RelayCommand(ExecuteQuery);
            this.StartingLocationResultCommand = new RelayCommand(SetStartingLocation);
        }

        #endregion

        #region Private methods

        private void OptionsPanelViewModel_MessageReceived(object sender, Core.Events.MessageReceivedEventArgs e)
        {
            if (e.CallerName == "SetStartingLocation")
            {
                StartingLocation = (City)e.Message;
            }
        }

        private void ExecuteQuery(object obj)
        {
            searchText = obj.ToString();

            Task.Run(() => new QueryExecutionTransferObject()
            {
                Argument = obj.ToString(),
                Result = cityDataService.GetCityListByName(obj.ToString())
            }).ContinueWith(t => VerifyAndApplyQueryResult(t.Result), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void VerifyAndApplyQueryResult(QueryExecutionTransferObject queryExecutionTransferObject)
        {
            if (searchText == queryExecutionTransferObject.Argument)
            {
                StartingLocationQueryResult = queryExecutionTransferObject;
            }
        }

        private void ChangeRefreshTime(object obj)
        {
            RefreshTime = (RefreshTime)(((int)obj + 1) * 15);
        }

        private void ChangeUnits(object obj)
        {
            ApplicationUnits = (Units)(int)obj;
        }

        private void ChangeLanguage(object obj)
        {
            ApplicationLanguage = (Language)(int)obj;
        }

        private void SetStartingLocation(object obj)
        {
            StartingLocation = (City)obj;
        }

        private void OnRefreshTimeChanged()
        {
            App.AppSettings.Interval = (int)refreshTime;
            Notify("RefreshTimeUpdated", refreshTime);
            SetUpdateConfigurationFlag();
        }

        private void OnApplicationUnitsChanged()
        {
            App.AppSettings.Units = appUnits;
            Notify("UnitsUpdated", appUnits);
            SetUpdateConfigurationFlag();
        }

        private void OnApplicationLanguageChanged(Language language)
        {
            App.AppSettings.Language = language;
            Notify("LanguageUpdated", appLanguage);
            SetUpdateConfigurationFlag();
        }

        private void OnStartinglocationChanged(City startingLocation)
        {
            App.AppSettings.CityId = startingLocation.Id.ToString();
            Notify("StartingLocationUpdated", startingLocation);
            SetUpdateConfigurationFlag();
        }

        private void SetUpdateConfigurationFlag()
        {
            if (!UpdateConfiguration)
            {
                UpdateConfiguration = true;
            }
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
