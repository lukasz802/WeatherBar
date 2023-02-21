using System.Collections.Generic;
using System.Windows.Input;
using WeatherBar.Application.Commands;
using WeatherBar.Application.Events;
using WeatherBar.Application.Events.Interfaces;
using WeatherBar.Model;
using WeatherBar.WpfApp.ViewModel.Templates;

namespace WeatherBar.WpfApp.ViewModel
{
    public class ForecastPanelViewModel : ViewModelBase, 
        IEventHandler<DailyForecastUpdatedEvent>, 
        IEventHandler<UnitsUpdatedEvent>,
        IEventHandler<LanguageUpdatedEvent>
    {
        #region Fields

        private List<HourlyForecast> dailyForecast;

        #endregion

        #region Public properties

        public ICommand ReturnToMainPanelCommand { get; private set; }

        public List<HourlyForecast> DailyForecast
        {
            get => dailyForecast ?? new List<HourlyForecast>() { HourlyForecast.Empty() };
            private set
            {
                dailyForecast = value;
                Notify(new DailyForecastUpdatedEvent(this, value));
            }
        }

        #endregion

        #region Constructors

        public ForecastPanelViewModel()
        {
            this.ReturnToMainPanelCommand = new RelayCommand(ReturnToMainPanel);
        }

        #endregion

        #region Public methods

        public void Handle(UnitsUpdatedEvent @event)
        {
            DailyForecast.ForEach(x => x.Units = @event.Content);

            DailyForecast = new List<HourlyForecast>(DailyForecast);
        }

        public void Handle(LanguageUpdatedEvent @event)
        {
            DailyForecast.ForEach(x => x.Language = @event.Content);

            DailyForecast = new List<HourlyForecast>(DailyForecast);
        }

        public void Handle(DailyForecastUpdatedEvent @event)
        {
            DailyForecast = @event.Content;
        }

        #endregion

        #region Private methods

        private void ReturnToMainPanel(object obj)
        {
            Notify(new ForecastPanelVisibilityChangedEvent(this, false));
        }

        #endregion
    }
}
