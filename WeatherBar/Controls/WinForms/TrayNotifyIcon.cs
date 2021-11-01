using System;
using System.Reflection;
using System.Windows;
using WeatherBar.ViewModel;

namespace WeatherBar.Controls.WinForms
{
    public class TrayNotifyIcon
    {
        #region Fields

        private static TrayNotifyIcon trayNotifyIconInstance;

        private System.Windows.Forms.NotifyIcon trayNotifyIcon;

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;

        private System.Windows.Forms.MouseEventHandler openToolStripMenuItemMouseEventHandler;

        private System.Windows.Forms.MouseEventHandler closeToolStripMenuItemMouseEventHandler;

        private Action refreshToolStripMenuItemAction;


        #endregion

        #region Properties

        public static TrayNotifyIcon TrayNotifyIconInstance
        {
            get
            {
                if (trayNotifyIconInstance == null)
                {
                    trayNotifyIconInstance = new TrayNotifyIcon();
                }

                return trayNotifyIconInstance;
            }
        }

        public bool IsVisible
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

        public Action RefreshToolStripMenuItemAction
        {
            get
            {
                return refreshToolStripMenuItemAction;
            }
            set
            {
                refreshToolStripMenuItemAction = value;
                UpdateRefreshToolStripMenuItemAction(value);
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

        private TrayNotifyIcon()
        {
            InitializeTrayIcon();
        }

        #endregion

        #region Public methods

        public void Update()
        {
            var text = !AppViewModel.Instance.IsReady ? (string)Application.Current.Resources["Updating"] : AppViewModel.Instance.IsConnected ?
                        $"{AppViewModel.Instance.CityName}, {AppViewModel.Instance.Country}\n{AppViewModel.Instance.Description}\n{(string)Application.Current.Resources["Temperature"]} " +
                        $"{AppViewModel.Instance.AvgTemp}/{AppViewModel.Instance.FeelTemp}{(string)Application.Current.Resources["TempUnit"]}\n" +
                        $"{(string)Application.Current.Resources["Update"]} {AppViewModel.Instance.UpdateTime}" : (string)Application.Current.Resources["NoConnectionServer"];

            SetNotifyIconText(text);
            trayNotifyIcon.Icon = new System.Drawing.Icon(AppResources.ResourceManager.GetIcon(!AppViewModel.Instance.IsReady ? "Update" : AppViewModel.Instance.IsConnected ? AppViewModel.Instance.Icon : string.Empty));
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
                Icon = new System.Drawing.Icon(AppResources.ResourceManager.GetIcon("Update")),
            };
        }

        private void UpdateOpenToolStripMenuEventHandler(System.Windows.Forms.MouseEventHandler newOpenToolStripMenuItemMouseEventHandler)
        {
            trayNotifyIcon.MouseClick += OpenToolStripMenuItemMouseEventHandler;
            contextMenuStrip.Items[0].Click += (s, e) =>
                newOpenToolStripMenuItemMouseEventHandler(s, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));
        }

        private void UpdateCloseToolStripMenuEventHandler(System.Windows.Forms.MouseEventHandler newCloseToolStripMenuItemMouseEventHandler)
        {
            contextMenuStrip.Items[3].Click += (s, e) =>
               newCloseToolStripMenuItemMouseEventHandler(s, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));
        }

        private void UpdateRefreshToolStripMenuItemAction(Action newRefreshToolStripMenuItemAction)
        {
            contextMenuStrip.Items[1].Click += (s, e) => newRefreshToolStripMenuItemAction();
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
