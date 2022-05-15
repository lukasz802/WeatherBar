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
            ViewModelManager.CreateViewModel<NotFoundPanelViewModel>(this);
        }
    }
}
