using System.Windows;
using WebApi;

namespace WeatherBar
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        private static WeatherApi client;

        #endregion

        #region Methods

        public static WeatherApi WebApiClient
        {
            get
            {
                if (client == null)
                {
                    client = new WeatherApi();
                }

                return client;
            }
        }

        #endregion
    }
}
