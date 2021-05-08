using WeatherBar.WebApi.Models.Interfaces;

namespace WeatherBar.WebApi.Models.DataTransferObjects
{
    public class HourlyForecast : IHourlyData
    {
        #region Properties

        public string Description { get; set; }

        public string CityName { get; set; }

        public int CityId { get; set; }

        public int AvgTemp { get; set; }

        public int FeelTemp { get; set; }

        public double SnowFall { get; set; }

        public double RainFall { get; set; }

        public string DayTime { get; set; }

        public string SunsetTime { get; set; }

        public string SunriseTime { get; set; }

        public string Country { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public int Pressure { get; set; }

        public int Humidity { get; set; }

        public int WindSpeed { get; set; }

        public int WindAngle { get; set; }

        public string Icon { get; set; }

        #endregion

        #region Constructors

        public HourlyForecast()
        {
            CityName = "Warszawa";
            Icon = "01d";
        }

        #endregion
    }
}
