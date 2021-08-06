namespace WeatherBar.WebApi.Models.Interfaces
{
    public interface IHourlyData : IWeatherData
    {
        string CityName { get; set; }

        int CityId { get; set; }

        int AvgTemp { get; set; }

        int FeelTemp { get; set; }

        double SnowFall { get; set; }

        double RainFall { get; set; }

        string DayTime { get; set; }

        string Date { get; set; }

        string WeekDay { get; set; }

        string SunsetTime { get; set; }

        string SunriseTime { get; set; }

        string Country { get; set; }

        string Longitude { get; set; }

        string Latitude { get; set; }

        int Pressure { get; set; }

        int Humidity { get; set; }

        int WindSpeed { get; set; }

        int WindAngle { get; set; }

        string Description { get; set; }

        string Icon { get; set; }
    }
}
