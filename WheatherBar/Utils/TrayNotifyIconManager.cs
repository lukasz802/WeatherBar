using AppData;
using System.Reflection;
using WeatherBar.ViewModels;

namespace WeatherBar.Utils
{
    public class TrayNotifyIconManager
    {
        #region Fields

        private static TrayNotifyIconManager trayNotifyIconInstance;

        private System.Windows.Forms.NotifyIcon trayNotifyIcon;

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;

        private System.Windows.Forms.MouseEventHandler openToolStripMenuItemMouseEventHandler;

        private System.Windows.Forms.MouseEventHandler closeToolStripMenuItemMouseEventHandler;

        private MainViewModel mainViewModelInstance;

        #endregion

        #region Properties

        public static TrayNotifyIconManager TrayNotifyIconInstance
        {
            get
            {
                if (trayNotifyIconInstance == null)
                {
                    trayNotifyIconInstance =  new TrayNotifyIconManager();
                }

                return trayNotifyIconInstance;
            }
        }

        public bool IsIconVisible
        {
            get
            {
                return trayNotifyIcon.Visible;
            }
            set
            {
                trayNotifyIcon.Visible = value;
            }
        }

        public MainViewModel MainViewModelInstance
        {
            get
            {
                return mainViewModelInstance;
            }
            set
            {
                mainViewModelInstance = value;
                UpdateMainViewModelInstance(value);
            }
        }

        public System.Windows.Forms.MouseEventHandler OpenToolStripMenuItemMouseEventHandler
        {
            get
            {
                return openToolStripMenuItemMouseEventHandler;
            }
            set
            {
                openToolStripMenuItemMouseEventHandler = value;
                UpdateOpenToolStripMenuEventHandler(value);
            }
        }

        public System.Windows.Forms.MouseEventHandler CloseToolStripMenuItemMouseEventHandler
        {
            get
            {
                return closeToolStripMenuItemMouseEventHandler;
            }
            set
            {
                closeToolStripMenuItemMouseEventHandler = value;
                UpdateCloseToolStripMenuEventHandler(value);
            }
        }

        #endregion

        #region Constructors

        private TrayNotifyIconManager()
        {
            InitializeTrayIcon();
        }

        #endregion

        #region Public methods

        public void Update()
        {
            var text = !MainViewModelInstance.IsReady ? "Aktualizowanie..." : MainViewModelInstance.IsConnected ?
                       $"{MainViewModelInstance.CityName}, {MainViewModelInstance.Country}\n{MainViewModelInstance.Description}\nTemperatura: " +
                       $"{MainViewModelInstance.AvgTemp}/{MainViewModelInstance.FeelTemp}°C\n" +
                       $"Zaktualizowano o: {MainViewModelInstance.UpdateTime}" : "Brak połączenia z serwerem Openweather.org";
            SetNotifyIconText(text);
            trayNotifyIcon.Icon = new System.Drawing.Icon(
                DataContainer.GetIcon(!MainViewModelInstance.IsReady ? "Update" : MainViewModelInstance.IsConnected ? MainViewModelInstance.Icon : string.Empty));
        }

        #endregion

        #region Private methods

        private void InitializeTrayIcon()
        {
            if (contextMenuStrip == null)
            {
                contextMenuStrip = PrepareContextMenu();
            }

            trayNotifyIcon = new System.Windows.Forms.NotifyIcon
            {
                ContextMenuStrip = contextMenuStrip,
                Visible = false,
                Icon = new System.Drawing.Icon(DataContainer.GetIcon("Update")),
            };
        }

        private void UpdateOpenToolStripMenuEventHandler(System.Windows.Forms.MouseEventHandler NewOpenToolStripMenuItemMouseEventHandler)
        {
            trayNotifyIcon.MouseClick += OpenToolStripMenuItemMouseEventHandler;
            contextMenuStrip.Items[0].Click += (s, e) =>
                NewOpenToolStripMenuItemMouseEventHandler(s, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));
        }

        private void UpdateCloseToolStripMenuEventHandler(System.Windows.Forms.MouseEventHandler NewCloseToolStripMenuItemMouseEventHandler)
        {
            contextMenuStrip.Items[3].Click += (s, e) =>
               NewCloseToolStripMenuItemMouseEventHandler(s, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));
        }

        private void UpdateMainViewModelInstance(MainViewModel newMainViewModel)
        {
            contextMenuStrip.Items[1].Click += (s, e) => MainViewModelInstance.Refresh(MainViewModelInstance.CityName);
        }

        private System.Windows.Forms.ContextMenuStrip PrepareContextMenu()
        {
            var openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem("Otwórz WeatherBar")
            {
                BackColor = System.Drawing.Color.Transparent,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold),
            };

            var updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem("Aktualizuj")
            {
                BackColor = System.Drawing.Color.Transparent,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular),
            };

            var closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem("Zakończ")
            {
                BackColor = System.Drawing.Color.Transparent,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular),
            };

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

        private void SetNotifyIconText(string text)
        {
            var type = typeof(System.Windows.Forms.NotifyIcon);
            var hiddenFieldFlag = BindingFlags.NonPublic | BindingFlags.Instance;
            type.GetField("text", hiddenFieldFlag).SetValue(trayNotifyIcon, text);

            if ((bool)type.GetField("added", hiddenFieldFlag).GetValue(trayNotifyIcon))
            {
                type.GetMethod("UpdateIcon", hiddenFieldFlag).Invoke(trayNotifyIcon, new object[] { true });
            }
        }

        #endregion
    }
}
