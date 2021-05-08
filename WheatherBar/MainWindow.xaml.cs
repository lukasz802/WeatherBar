using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WeatherBar.Utils;
using WeatherBar.ViewModels;

namespace WeatherBar
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private MainViewModel viewModel = new MainViewModel();
        
        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => this.DataContext = viewModel;
            InitializeTrayIcon();
            this.Loaded += (s, e) => MainPanelFrame.DataContext = viewModel;
            this.Loaded += (s, e) => ConnectionFailedFrame.DataContext = viewModel;
            this.Loaded += (s, e) => ForecastFrame.DataContext = viewModel;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        #endregion

        #region Private methods

        private void InitializeTrayIcon()
        {
            TrayNotifyIconManager.TrayNotifyIconInstance.MainViewModelInstance = viewModel;
            TrayNotifyIconManager.TrayNotifyIconInstance.OpenToolStripMenuItemMouseEventHandler = TrayNotifyIcon_MouseClick;
            TrayNotifyIconManager.TrayNotifyIconInstance.CloseToolStripMenuItemMouseEventHandler = (s, e) => MenuBarButton_Click(CloseButton, null);
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            TrayNotifyIconManager.TrayNotifyIconInstance.Update();
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
                TrayNotifyIconManager.TrayNotifyIconInstance.IsVisible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                TrayNotifyIconManager.TrayNotifyIconInstance.IsVisible = false;
                this.ShowInTaskbar = true;
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };

            timer.Start();
            timer.Tick += (s, a) =>
            {
                timer.Stop();

                var prevButton = ((FrameworkElement)MainPanelFrame.Content).FindName("PreviousButton") as Button;

                if (prevButton.IsEnabled)
                {
                    MouseButtonEventArgs arg = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                    {
                        RoutedEvent = Button.ClickEvent
                    };

                    prevButton.RaiseEvent(arg);
                }
            };
        }

        #endregion
    }
}
