using System.Windows.Input;
using WeatherBar.Core.Commands;
using WeatherBar.Core.Enums;
using WeatherBar.Core.Events;
using WeatherBar.ViewModel.Templates;

namespace WeatherBar.ViewModel
{
    public class NotFoundPanelViewModel : ViewModelBase
    {
        #region Fields

        private AppStatus appStatus;

        #endregion

        #region Public properties

        public ICommand ReturnToMainPanelCommand { get; private set; }

        public AppStatus AppStatus
        {
            get => appStatus;
            set
            {
                appStatus = value;
                Notify();
            }
        }

        #endregion

        #region Constructors

        public NotFoundPanelViewModel()
        {
            this.ReturnToMainPanelCommand = new RelayCommand(ReturnToMainPanel);
        }

        #endregion

        #region Private methods

        private void ReturnToMainPanel(object obj)
        {
            this.AppStatus = AppStatus.LoadingResource;
            EventDispatcher.RaiseEventWithDelay(() => this.AppStatus = AppStatus.Ready, 500);
        }

        #endregion
    }
}
