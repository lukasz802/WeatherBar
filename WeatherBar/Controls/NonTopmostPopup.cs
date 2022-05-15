using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using WeatherBar.Utils;

namespace WeatherBar.Controls
{
    public class NonTopmostPopup : Popup
    {
        #region Fields

        private bool alreadyLoaded;

        private Window parentWindow;

        #endregion

        #region Constructors

        public NonTopmostPopup()
        {
            Loaded += OnPopupLoaded;
            Loaded += (s, e) => AutoRepositionPopupBehavior();
            Unloaded += OnPopupUnloaded;
        }

        #endregion

        #region Public methods

        protected override void OnOpened(EventArgs e)
        {
            SetNonTopmostState();
            base.OnOpened(e);
        }

        #endregion

        #region Private methods

        private void AutoRepositionPopupBehavior()
        {
            UserControl parent = GlobalUtils.FindVisualParent<UserControl>(this);
            Window window = Window.GetWindow(parent);

            if (window != null || this == null)
            {
                window.LocationChanged += (s, t) =>
                {
                    var offset = HorizontalOffset;

                    HorizontalOffset = offset + 1;
                    HorizontalOffset = offset;
                };
            }

            PlacementTarget = parent;
        }

        private void OnPopupLoaded(object sender, RoutedEventArgs e)
        {
            if (alreadyLoaded)
            {
                return;
            }

            alreadyLoaded = true;

            if (Child != null)
            {
                Child.AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(OnChildPreviewMouseLeftButtonDown), true);
            }

            parentWindow = Window.GetWindow(this);

            if (parentWindow == null)
            {
                return;
            }

            parentWindow.Activated += OnParentWindowActivatedDeactivated;
            parentWindow.Deactivated += OnParentWindowActivatedDeactivated;
        }

        private void OnPopupUnloaded(object sender, RoutedEventArgs e)
        {
            if (parentWindow == null)
            {
                return;
            }

            parentWindow.Activated -= OnParentWindowActivatedDeactivated;
            parentWindow.Deactivated -= OnParentWindowActivatedDeactivated;
        }

        private void OnParentWindowActivatedDeactivated(object sender, EventArgs e)
        {
            SetNonTopmostState();
        }

        private void OnChildPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetNonTopmostState();

            if (!parentWindow.IsActive)
            {
                parentWindow.Activate();
            }
        }

        private void SetNonTopmostState()
        {
            if (Child == null)
            {
                return;
            }

            var hwndSource = PresentationSource.FromVisual(Child) as HwndSource;

            if (hwndSource == null)
            {
                return;
            }

            var hwnd = hwndSource.Handle;

            if (!GetWindowRect(hwnd, out RECT rect))
            {
                return;
            }

            SetWindowPos(hwnd, HWND_BOTTOM, rect.Left, rect.Top, (int)Width, (int)Height, TOPMOST_FLAGS);
            SetWindowPos(hwnd, HWND_TOP, rect.Left, rect.Top, (int)Width, (int)Height, TOPMOST_FLAGS);
            SetWindowPos(hwnd, HWND_NOTOPMOST, rect.Left, rect.Top, (int)Width, (int)Height, TOPMOST_FLAGS);
        }

        #endregion

        #region External imports & definitions

        #region Fields

        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        private static readonly IntPtr HWND_TOP = new IntPtr(0);

        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        private const uint SWP_NOSIZE = 0x0001;

        private const uint SWP_NOMOVE = 0x0002;

        private const uint SWP_NOREDRAW = 0x0008;

        private const uint SWP_NOACTIVATE = 0x0010;

        private const uint SWP_NOOWNERZORDER = 0x0200;

        private const uint SWP_NOSENDCHANGING = 0x0400;

        private const uint TOPMOST_FLAGS =
            SWP_NOACTIVATE | SWP_NOOWNERZORDER | SWP_NOSIZE | SWP_NOMOVE | SWP_NOREDRAW | SWP_NOSENDCHANGING;

        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;

            public int Top;

            public int Right;

            public int Bottom;
        }

        #endregion

        #region Methods

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        #endregion

        #endregion
    }
}