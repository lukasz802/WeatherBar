using System.Windows.Controls;
using WeatherBar.Core;
using WeatherBar.ViewModel;

namespace WeatherBar.View.Pages
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
            ViewModelManager.CreateViewModel<ForecastPanelViewModel>(this);
        }

        #endregion
    }
}
