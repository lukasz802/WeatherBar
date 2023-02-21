using System.Windows.Input;
using WeatherBar.Application.Commands;
using WeatherBar.Application.Dispatchers;
using WeatherBar.Application.Events;
using WeatherBar.Application.Events.Interfaces;
using WeatherBar.Model.Enums;
using WeatherBar.WpfApp.ViewModel.Templates;

namespace WeatherBar.WpfApp.ViewModel
{
    public class NotFoundPanelViewModel : ViewModelBase, IEventHandler<AppStatusChangedEvent>
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
                Notify(new AppStatusChangedEvent(this, value));
            }
        }

        #endregion

        #region Constructors

        public NotFoundPanelViewModel()
        {
            this.ReturnToMainPanelCommand = new RelayCommand(ReturnToMainPanel);
        }

        #endregion

        #region Public methods

        public void Handle(AppStatusChangedEvent @event)
        {
            AppStatus = @event.Content;
        }

        #endregion

        #region Private methods

        private void ReturnToMainPanel(object obj)
        {
            this.AppStatus = AppStatus.LoadingResource;
            EventDispatcher.RaiseEventWithDelay(() => this.AppStatus = AppStatus.Ready, 200);
        }

        #endregion
    }
}
