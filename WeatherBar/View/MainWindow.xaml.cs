using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeatherBar.Controls.WinForms;
using WeatherBar.Core;
using WeatherBar.Core.Events;
using WeatherBar.ViewModel;

namespace WeatherBar.View
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private readonly MainWindowViewModel viewModel;

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            viewModel = ViewModelManager.CreateViewModel<MainWindowViewModel>(this);
            InitializeTrayIcon();
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        #endregion

        #region Private methods

        private void InitializeTrayIcon()
        {
            TrayNotifyIcon.Instance.RefreshToolStripMenuItemAction = viewModel.Refresh;
            TrayNotifyIcon.Instance.OpenToolStripMenuItemMouseEventHandler = TrayNotifyIcon_MouseClick;
            TrayNotifyIcon.Instance.CloseToolStripMenuItemMouseEventHandler = (s, e) => MenuBarButton_Click(CloseButton, null);
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsReady" && !viewModel.IsReady)
            {
                TrayNotifyIcon.Instance.Update((string)Application.Current.Resources["Updating"], "Update");
            }
        }

        private void TrayNotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.WindowState = WindowState.Normal;
                this.Activate();
            }
        }

        private void MenuBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == CloseButton)
            {
                this.Close();
            }
            else
            {
                this.WindowState = WindowState.Minimized;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                TrayNotifyIcon.Instance.IsVisible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                TrayNotifyIcon.Instance.IsVisible = false;
                this.ShowInTaskbar = true;
            }
        }

        private void ButtonPressAction()
        {
            var prevButton = ((FrameworkElement)MainPanelFrame.Content).FindName("PreviousButton") as Button;

            if (prevButton.IsEnabled)
            {
                MouseButtonEventArgs arg = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                {
                    RoutedEvent = Button.ClickEvent
                };

                prevButton.RaiseEvent(arg);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            EventDispatcher.RaiseEventWithDelay(this.ButtonPressAction, 200);
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            EventDispatcher.RaiseEventWithDelay(() =>
            {
                RefreshButton.IsEnabled = OpenSiteButton.IsEnabled = !viewModel.IsOptionsPanelVisible;
            }, viewModel.IsOptionsPanelVisible ? 350 : 0);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                this.Close();
            }
        }

        #endregion
    }
}
