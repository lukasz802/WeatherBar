using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeatherBar.Controls;

namespace WeatherBar.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy MainPanelPage.xaml
    /// </summary>
    public partial class MainPanelPage : Page
    {
        #region Constructors

        public MainPanelPage()
        {
            InitializeComponent();
        }

        #endregion

        #region Private methods

        private void CityTextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ForecastDataWrapPanel.Width =
                App.Current.MainWindow.ActualWidth - CityTextBlock.ActualWidth - SplitGrid.Margin.Left - SplitGrid.Margin.Right - 12;
        }

        private void SearchUserControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Back:
                case Key.Space:
                case Key.Enter:
                case Key.Tab:
                case Key.Left:
                case Key.Right:
                    break;
                default:
                    if (e.Key.ToString().Length != 1)
                    {
                        e.Handled = true;
                    }

                    break;
            }
        }

        private void SearchUserControl_SearchClick(object sender, RoutedEventArgs e)
        {
            ((SearchTextBox)sender).SearchTextBoxControl.Clear();
        }

        #endregion
    }
}
