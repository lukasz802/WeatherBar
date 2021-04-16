using System.Windows;
using WeatherBar.WebApi;
using WeatherBar.WebApi.Models.Enums;

namespace WeatherBar
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        private const string UniqueApiKey = "c5976f0996947c1488798209b0bc3f77";

        private static WeatherApi client;

        #endregion

        #region Methods

        public static WeatherApi WebApiClient
        {
            get
            {
                if (client == null)
                {
                    client = new WeatherApi(UniqueApiKey, Units.Metric);
                }

                return client;
            }
        }

        #endregion
    }
}
