﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeatherBar.Controls.WinForms;
using WeatherBar.Core;
using WeatherBar.ViewModel;

namespace WeatherBar.View
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => this.DataContext = AppViewModel.Instance;
            InitializeTrayIcon();
            AppViewModel.Instance.PropertyChanged += ViewModel_PropertyChanged;
        }

        #endregion

        #region Private methods

        private void InitializeTrayIcon()
        {
            TrayNotifyIcon.TrayNotifyIconInstance.RefreshToolStripMenuItemAction = AppViewModel.Instance.Refresh;
            TrayNotifyIcon.TrayNotifyIconInstance.OpenToolStripMenuItemMouseEventHandler = TrayNotifyIcon_MouseClick;
            TrayNotifyIcon.TrayNotifyIconInstance.CloseToolStripMenuItemMouseEventHandler = (s, e) => MenuBarButton_Click(CloseButton, null);
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            TrayNotifyIcon.TrayNotifyIconInstance.Update();
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
            EventDispatcher.RaiseEventWithDelay(this.ButtonPressAction, 200);
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            EventDispatcher.RaiseEventWithDelay(() =>
            {
                RefreshButton.IsEnabled = OpenSiteButton.IsEnabled = !AppViewModel.Instance.IsOptionsPanelVisible;
            }, AppViewModel.Instance.IsOptionsPanelVisible ? 350 : 0);
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
