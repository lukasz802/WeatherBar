using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using WeatherBar.Controls.WinForms;
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
            TrayNotifyIcon.TrayNotifyIconInstance.MainViewModelInstance = viewModel;
            TrayNotifyIcon.TrayNotifyIconInstance.OpenToolStripMenuItemMouseEventHandler = TrayNotifyIcon_MouseClick;
            TrayNotifyIcon.TrayNotifyIconInstance.CloseToolStripMenuItemMouseEventHandler = (s, e) => MenuBarButton_Click(CloseButton, null);
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            TrayNotifyIcon.TrayNotifyIconInstance.Update();

            if (e.PropertyName == "HasStarted" && viewModel.HasStarted)
            {
                SharedFunctions.RaiseEventWithDelay(LoadingFrameVisibilityVerification, 1000);
            }
        }

        private void LoadingFrameVisibilityVerification()
        {
            if (LoadingFrame.Visibility == Visibility.Visible)
            {
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = new Duration(TimeSpan.Parse("0:0:0.2"))
                };

                LoadingFrame.BeginAnimation(OpacityProperty, animation);
                SharedFunctions.RaiseEventWithDelay(() =>
                {
                    LoadingFrame.Visibility = Visibility.Hidden;
                }, 300);
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
                TrayNotifyIcon.TrayNotifyIconInstance.IsVisible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                TrayNotifyIcon.TrayNotifyIconInstance.IsVisible = false;
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
            SharedFunctions.RaiseEventWithDelay(ButtonPressAction, 200);
        }

        #endregion
    }
}
