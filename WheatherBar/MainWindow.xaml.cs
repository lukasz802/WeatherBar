using AppData;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using WeatherBar.ViewModels;

namespace WeatherBar
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private System.Windows.Forms.NotifyIcon trayNotifyIcon;

        private MainViewModel viewModel = new MainViewModel();
        
        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            InitializeTrayIcon();
            this.Loaded += (s, e) => this.DataContext = viewModel;
            this.Loaded += (s, e) => MainPanelFrame.DataContext = viewModel;
            this.Loaded += (s, e) => ConnectionFailedFrame.DataContext = viewModel;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        #endregion

        #region Private methods

        private void SetNotifyIconText(string text)
        {
            Type type = typeof(System.Windows.Forms.NotifyIcon);
            BindingFlags hiddenFieldFlag = BindingFlags.NonPublic | BindingFlags.Instance;
            type.GetField("text", hiddenFieldFlag).SetValue(trayNotifyIcon, text);

            if ((bool)type.GetField("added", hiddenFieldFlag).GetValue(trayNotifyIcon))
            {
                type.GetMethod("UpdateIcon", hiddenFieldFlag).Invoke(trayNotifyIcon, new object[] { true });
            }
        }

        private void InitializeTrayIcon()
        {
            trayNotifyIcon = new System.Windows.Forms.NotifyIcon
            {
                ContextMenuStrip = PrepareContextMenu(),
                Visible = false,
            };
            trayNotifyIcon.MouseClick += TrayNotifyIcon_MouseClick;
        }

        private System.Windows.Forms.ContextMenuStrip PrepareContextMenu()
        {
            var openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem("Otwórz WeatherBar")
            {
                BackColor = System.Drawing.Color.Transparent,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold),
            };
            openToolStripMenuItem.Click += (s, e) => 
                TrayNotifyIcon_MouseClick(s, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1 ,0, 0, 0));

            var updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem("Aktualizuj")
            {
                BackColor = System.Drawing.Color.Transparent,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular),
            };
            updateToolStripMenuItem.Click += (s, e) => viewModel.Refresh();

            var closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem("Zakończ")
            {
                BackColor = System.Drawing.Color.Transparent,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular),
            };
            closeToolStripMenuItem.Click += (s, e) => MenuBarButton_Click(CloseButton, null);

            System.Windows.Forms.ContextMenuStrip contextMenu = new System.Windows.Forms.ContextMenuStrip
            {
                BackColor = System.Drawing.Color.Transparent,
            };
            contextMenu.Items.Add(openToolStripMenuItem);
            contextMenu.Items.Add(updateToolStripMenuItem);
            contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator()
            {
                BackColor = System.Drawing.Color.Transparent,
            });
            contextMenu.Items.Add(closeToolStripMenuItem);

            return contextMenu;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var text = viewModel.IsReady ? $"{viewModel.CityName}, {viewModel.Country}\n{viewModel.Description}\nTemperatura: {viewModel.AvgTemp}/{viewModel.FeelTemp}°C\n" +
                    $"Zaktualizowano o: {viewModel.UpdateTime}" : "Brak połączenia z serwerem Openweather.org";
            SetNotifyIconText(text);
            trayNotifyIcon.Icon = new System.Drawing.Icon(DataContainer.GetIcon(viewModel.IsReady ? viewModel.Icon : string.Empty));
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
                trayNotifyIcon.Visible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                trayNotifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
        }

        #endregion
    }
}
