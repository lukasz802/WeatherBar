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
            ForecastTypeComboBox.SelectionChanged += ForecastTypeComboBox_Selected;
        }

        #endregion

        #region Private methods

        private void CityTextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WeatherDataWrapPanel.Width =
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
                case Key.Delete:
                case Key.OemMinus:
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

        private void ForecastListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((CommandListBox)sender).UnselectAll();
        }

        private void ListBox_PreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void HourlyListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;

            if (sender == NextButton)
            {
                PreviousButton.IsEnabled = true;
            }
            else if (sender == PreviousButton)
            {
                NextButton.IsEnabled = true;
            }
        }

        private void ForecastTypeComboBox_Selected(object sender, RoutedEventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex == 1)
            {
                MouseButtonEventArgs arg = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                {
                    RoutedEvent = Button.ClickEvent
                };

                PreviousButton.RaiseEvent(arg);
            }
        }

        #endregion
    }
}
