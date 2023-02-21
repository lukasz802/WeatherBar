using System.Windows.Controls;
using WeatherBar.WpfApp.Managers;
using WeatherBar.WpfApp.ViewModel;

namespace WeatherBar.WpfApp.View.Pages
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
