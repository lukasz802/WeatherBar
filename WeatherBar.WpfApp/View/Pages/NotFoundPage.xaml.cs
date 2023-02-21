using System.Windows.Controls;
using WeatherBar.WpfApp.Managers;
using WeatherBar.WpfApp.ViewModel;

namespace WeatherBar.WpfApp.View.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy NotFoundPage.xaml
    /// </summary>
    public partial class NotFoundPage : Page
    {
        public NotFoundPage()
        {
            InitializeComponent();

            var viewModel = new NotFoundPanelViewModel();

            ViewModelManager.Register(viewModel, this);

            this.Loaded += (s, e) => this.DataContext = viewModel;
        }
    }
}
