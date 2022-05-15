using System.Windows.Input;
using WeatherBar.Core.Commands;
using WeatherBar.ViewModel.Templates;

namespace WeatherBar.ViewModel
{
    public class ConnectionFailedPanelViewModel : ViewModelBase
    {
        #region Properties

        public ICommand RefreshCommand { get; private set; }

        #endregion

        #region Constructors

        public ConnectionFailedPanelViewModel()
        {
            this.RefreshCommand = new RelayCommand(Refresh);
        }

        #endregion

        #region Private methods

        private void Refresh()
        {
            Notify("TryRefreshAgain");
        }

        #endregion
    }
}
