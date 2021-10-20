using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WeatherBar.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy CityPopupTextBox.xaml
    /// </summary>
    public partial class CityPopupTextBox : UserControl
    {
        #region Fields

        private Popup popup;

        #endregion

        #region Properties implementation

        public string CityName
        {
            get { return (string)GetValue(CityNameProperty); }
            set { SetValue(CityNameProperty, value); }
        }

        public static readonly DependencyProperty CityNameProperty =
            DependencyProperty.Register("CityName", typeof(string), typeof(CityPopupTextBox));

        public string Latitude
        {
            get { return (string)GetValue(LatitudeProperty); }
            set { SetValue(LatitudeProperty, value); }
        }

        public static readonly DependencyProperty LatitudeProperty =
            DependencyProperty.Register("Latitude", typeof(string), typeof(CityPopupTextBox));

        public string Longtitude
        {
            get { return (string)GetValue(LongtitudeProperty); }
            set { SetValue(LongtitudeProperty, value); }
        }

        public static readonly DependencyProperty LongtitudeProperty =
            DependencyProperty.Register("Longtitude", typeof(string), typeof(CityPopupTextBox));

        public string CityId
        {
            get { return (string)GetValue(CityIdProperty); }
            set { SetValue(CityIdProperty, value); }
        }

        public static readonly DependencyProperty CityIdProperty =
            DependencyProperty.Register("CityId", typeof(string), typeof(CityPopupTextBox));


        public string Country
        {
            get { return (string)GetValue(CountryProperty); }
            set { SetValue(CountryProperty, value); }
        }

        public static readonly DependencyProperty CountryProperty =
            DependencyProperty.Register("Country", typeof(string), typeof(CityPopupTextBox));

        #endregion

        #region Private properties

        private Popup Popup
        {
            get
            {
                if (popup == null)
                {
                    popup = CityPopupControlButton.Template.FindName("Popup", CityPopupControlButton) as Popup;
                }

                return popup;
            }
        }

        #endregion

        #region Constructors

        public CityPopupTextBox()
        {
            InitializeComponent();
            CityPopupControlButton.LostFocus += CityPopupControlButton_LostFocus;    
        }

        #endregion

        #region Private methods

        private void CityPopupControlButton_Click(object sender, RoutedEventArgs e)
        {
            if (Popup != null)
            {
                Popup.Width = ActualWidth;
                Popup.Height = ActualHeight;
                Popup.IsOpen = true;
            }
        }

        private void CityPopupControlButton_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Popup != null)
            {
                Popup.IsOpen = false;
            }
        }

        #endregion
    }
}
