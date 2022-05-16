using System.Collections.Generic;
using System.Windows.Input;
using WeatherBar.Core.Commands;
using WeatherBar.Core.Events.Args;
using WeatherBar.Model.Interfaces;
using WeatherBar.ViewModel.Templates;

namespace WeatherBar.ViewModel
{
    public class ForecastPanelViewModel : ViewModelBase
    {
        #region Fields

        private bool isForecastPanelVisible;

        private List<IHourlyData> dailyForecast;

        #endregion

        #region Public properties

        public ICommand ReturnToMainPanelCommand { get; private set; }

        public bool IsForecastPanelVisible
        {
            get => isForecastPanelVisible;
            set
            {
                isForecastPanelVisible = value;
                Notify();
            }
        }

        public List<IHourlyData> DailyForecast
        {
            get => dailyForecast ?? new List<IHourlyData>() { Model.HourlyForecast.Empty() };
            private set
            {
                dailyForecast = value;
                Notify();
            }
        }

        #endregion

        #region Constructors

        public ForecastPanelViewModel()
        {
            this.AutomaticallyApplyReceivedChanges = true;
            this.IncludeOnlyPublicChanges = false;
            this.MessageReceived += DailyForecastViewModel_MessageReceived;
            this.ReturnToMainPanelCommand = new RelayCommand(ReturnToMainPanel);
        }

        #endregion

        #region Private methods

        private void DailyForecastViewModel_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.CallerName == "DailyForecastChanged")
            {
                var input = (List<IHourlyData>)e.Message;

                DailyForecast = input;
            }
        }

        private void ReturnToMainPanel(object obj)
        {
            this.IsForecastPanelVisible = false;
        }

        #endregion
    }
}
