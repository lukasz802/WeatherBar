using System.Windows.Controls;
using WeatherBar.WpfApp.Managers;
using WeatherBar.WpfApp.ViewModel;

namespace WeatherBar.WpfApp.View.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy ForecastPage.xaml
    /// </summary>
    public partial class ForecastPage : Page
    {
        #region Constructors

        public ForecastPage()
        {
            InitializeComponent();

            var viewModel = new ForecastPanelViewModel();

            ViewModelManager.Register(viewModel, this);

            this.Loaded += (s, e) => this.DataContext = viewModel;
        }

        #endregion
    }
}
