using System.Windows.Controls;
using WeatherBar.Core;
using WeatherBar.ViewModel;

namespace WeatherBar.View.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy ConnectionFailedPage.xaml
    /// </summary>
    public partial class ConnectionFailedPage : Page
    {
        public ConnectionFailedPage()
        {
            InitializeComponent();

            var viewModel = new ConnectionFailedPanelViewModel();

            ViewModelManager.Register(viewModel, this);

            this.Loaded += (s, e) => this.DataContext = viewModel;
        }
    }
}
