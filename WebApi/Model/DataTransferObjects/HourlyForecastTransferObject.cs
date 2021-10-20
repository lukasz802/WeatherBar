using System;
using WebApi.Model.Interfaces;

namespace WebApi.Model.DataTransferObjects
{
    public class HourlyForecastTransferObject : IWeatherData
    {
        #region Properties

        public string Description { get; set; }

        public string DescriptionId { get; set; }

        public string CityName { get; set; }

        public string CityId { get; set; }

        public double AvgTemp { get; set; }

        public double FeelTemp { get; set; }

        public double SnowFall { get; set; }

        public double RainFall { get; set; }

        public DateTime Date { get; set; }

        public long DayTime { get; set; }

        public int SunsetTime { get; set; }

        public int SunriseTime { get; set; }

        public string Country { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public int Pressure { get; set; }

        public int Humidity { get; set; }

        public double WindSpeed { get; set; }

        public int WindAngle { get; set; }

        public string Icon { get; set; }


        #endregion
    }
}
