using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeatherBar.Controls;
using WeatherBar.Core;
using WeatherBar.ViewModels;

namespace WeatherBar.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy MainPanelPage.xaml
    /// </summary>
    public partial class MainPanelPage : Page
    {
        #region Fields

        private MainViewModel viewModel;

        #endregion

        #region Constructors

        public MainPanelPage()
        {
            InitializeComponent();
            ForecastTypeComboBox.SelectionChanged += ForecastTypeComboBox_Selected;
            this.Loaded += (s, e) =>
            {
                viewModel = (MainViewModel)(Application.Current.MainWindow.FindName("MainPanelFrame") as Frame).DataContext;
                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            };
        }

        #endregion

        #region Private methods

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (viewModel.HasStarted && e.PropertyName == "IsReady" && viewModel.IsReady)
            {
                EventDispatcher.RaiseEventWithDelay(() =>
                {
                    SearchUserControl.Focusable = true;
                    SearchUserControl.SearchTextBoxControl.Focus();
                    SearchUserControl.Focusable = false;
                }, 300);
            }
        }

        private void CityTextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WeatherDataWrapPanel.Width =
                Application.Current.MainWindow.ActualWidth - CityTextBlock.ActualWidth - SplitGrid.Margin.Left - SplitGrid.Margin.Right - 12;
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
                case Key.Up:
                case Key.Down:
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
            if (!string.IsNullOrEmpty(((SearchTextBox)sender).Text))
            {
                ((SearchTextBox)sender).SearchTextBoxControl.Clear();
                EventDispatcher.RaiseEventWithDelay(() => ButtonPressAction(PreviousButton), 200);
                EventDispatcher.RaiseEventWithDelay(() => viewModel.IsForecastPanelVisible = false, 50);
            }
        }

        private void ListBox_PreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
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
            EventDispatcher.RaiseEventWithDelay(() => ButtonPressAction(PreviousButton), 400);
        }

        private void ButtonPressAction(Button button)
        {
            if (button.IsEnabled)
            {
                MouseButtonEventArgs arg = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                {
                    RoutedEvent = Button.ClickEvent
                };

                button.RaiseEvent(arg);
            }
        }

        private void ForecastListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((ListBox)sender).UnselectAll();
        }

        #endregion
    }
}
