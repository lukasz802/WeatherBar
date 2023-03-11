using System;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
using WeatherBar.AppResources;
using System.Collections.Generic;
using System.Linq;
using WeatherBar.Model.Enums;

namespace WeatherBar.WpfApp.Controls.WinForms
{
    public class TrayNotifyIcon
    {
        #region Fields

        private static TrayNotifyIcon instance;

        private readonly Dictionary<string, ToolStripMenuItem> contextMenuCache = new Dictionary<string, ToolStripMenuItem>();

        private NotifyIcon trayNotifyIcon;

        private ContextMenuStrip contextMenuStrip;

        private MouseEventHandler openToolStripMenuItemMouseEventHandler;

        private MouseEventHandler closeToolStripMenuItemMouseEventHandler;

        private Action refreshToolStripMenuItemAction;

        #endregion

        #region Properties

        public static TrayNotifyIcon Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TrayNotifyIcon();
                }

                return instance;
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

        public MouseEventHandler OpenToolStripMenuItemMouseEventHandler
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

        public MouseEventHandler CloseToolStripMenuItemMouseEventHandler
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

        public void Update(string text, string iconId)
        {
            UpdateNotifyIconText(text);

            trayNotifyIcon.Icon = ResourceManager.GetIcon(iconId);
        }

        public void ChangeContextMenuLanguage(Dictionary<string, string> resourceDictionary)
        {
            foreach (string key in resourceDictionary.Keys)
            {
                if (contextMenuCache.Keys.Contains(key))
                {
                    contextMenuCache[key].Text = resourceDictionary[key];
                }
            }
        }

        #endregion

        #region Private methods

        private void InitializeTrayIcon()
        {
            contextMenuStrip = PrepareContextMenu();
            trayNotifyIcon = new NotifyIcon
            {
                ContextMenuStrip = contextMenuStrip,
                Visible = false,
                Icon = ResourceManager.GetIcon("Update"),
            };

            Dictionary<string, string> resourceDictionary = ResourceManager.GetLanguage(App.AppSettings.Language);

            ChangeContextMenuLanguage(resourceDictionary);
        }

        private void UpdateOpenToolStripMenuEventHandler(MouseEventHandler newOpenToolStripMenuItemMouseEventHandler)
        {
            trayNotifyIcon.MouseClick += OpenToolStripMenuItemMouseEventHandler;
            contextMenuStrip.Items[0].Click += (s, e) =>
                newOpenToolStripMenuItemMouseEventHandler(s, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        }

        private void UpdateCloseToolStripMenuEventHandler(MouseEventHandler newCloseToolStripMenuItemMouseEventHandler)
        {
            contextMenuStrip.Items[3].Click += (s, e) =>
               newCloseToolStripMenuItemMouseEventHandler(s, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        }

        private void UpdateRefreshToolStripMenuItemAction(Action newRefreshToolStripMenuItemAction)
        {
            contextMenuStrip.Items[1].Click += (s, e) => newRefreshToolStripMenuItemAction();
        }

        private ContextMenuStrip PrepareContextMenu()
        {
            var openToolStripMenuItem = new ToolStripMenuItem()
            {
                Name = "OpenMenuItem",
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
            };
            var updateToolStripMenuItem = new ToolStripMenuItem()
            {
                Name = "UpdateMenuItem",
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
            };
            var quitToolStripMenuItem = new ToolStripMenuItem()
            {
                Name = "QuitMenuItem",
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
            };

            ContextMenuStrip contextMenu = new ContextMenuStrip
            {
                BackColor = Color.Transparent,
            };

            contextMenu.Items.Add(openToolStripMenuItem);
            contextMenu.Items.Add(updateToolStripMenuItem);
            contextMenu.Items.Add(new ToolStripSeparator()
            {
                BackColor = Color.Transparent,
            });
            contextMenu.Items.Add(quitToolStripMenuItem);

            PrepareContextMenuCache(contextMenu);

            return contextMenu;
        }

        private void PrepareContextMenuCache(ContextMenuStrip contextMenu)
        {
            foreach (ToolStripItem stripItem in contextMenu.Items)
            {
                var menuItem = stripItem as ToolStripMenuItem;

                if (menuItem != null)
                {
                    contextMenuCache.Add(stripItem.Name, menuItem);
                }
            }
        }

        private void UpdateNotifyIconText(string text)
        {
            var type = typeof(NotifyIcon);
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
