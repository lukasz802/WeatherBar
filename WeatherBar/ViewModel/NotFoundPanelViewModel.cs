using System.Windows.Input;
using WeatherBar.Core.Commands;
using WeatherBar.ViewModel.Templates;

namespace WeatherBar.ViewModel
{
    public class NotFoundPanelViewModel : ViewModelBase
    {
        #region Fields

        private bool resourceFounded;

        #endregion

        #region Public properties

        public ICommand ReturnToMainPanelCommand { get; private set; }

        public bool ResourceFounded
        {
            get => resourceFounded;
            private set
            {
                resourceFounded = value;
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
            this.ResourceFounded = true;
        }

        #endregion
    }
}
