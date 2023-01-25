using System.Windows.Controls;
using WeatherBar.Core;
using WeatherBar.ViewModel;

namespace WeatherBar.View.Pages
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
