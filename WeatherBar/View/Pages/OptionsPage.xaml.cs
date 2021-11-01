using System.Windows.Controls;
using System.Windows.Input;

namespace WeatherBar.View.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy OptionsPage.xaml
    /// </summary>
    public partial class OptionsPage : Page
    {
        #region Constructors

        #region Constructors

        public OptionsPage()
        {
            InitializeComponent();
        }

        #endregion

        #endregion

        #region Private methods

        private void CityPopupTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    //SearchUserControl_SearchClick(sender, null);
                    break;
                case Key.Back:
                case Key.Space:
                case Key.Tab:
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                case Key.Delete:
                case Key.Escape:
                case Key.OemMinus:
                    break;
                default:
                    if (e.Key.ToString().Length != 1)
                    {
                        e.Handled = true;
                    }
                    break;
            }
        }

        #endregion
    }
}
