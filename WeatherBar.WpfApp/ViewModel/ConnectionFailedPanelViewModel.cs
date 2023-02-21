using System.Windows.Input;
using WeatherBar.Application.Commands;
using WeatherBar.Application.Events;
using WeatherBar.WpfApp.ViewModel.Templates;

namespace WeatherBar.WpfApp.ViewModel
{
    public class ConnectionFailedPanelViewModel : ViewModelBase
    {
        #region Properties

        public ICommand RefreshCommand { get; private set; }

        #endregion

        #region Constructors

        public ConnectionFailedPanelViewModel()
        {
            this.RefreshCommand = new RelayCommand(RefreshWeatherData);
        }

        #endregion

        #region Private methods

        private void RefreshWeatherData()
        {
            Notify(new WeatherDataRefreshedEvent(this));
        }

        #endregion
    }
}
