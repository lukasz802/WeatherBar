
namespace WeatherBar.Model.Interfaces
{
    public interface IHourlyData : IMultiLanguage, IUnits, IClonable<IHourlyData>
    {
        string CityName { get; }

        string CityId { get; }

        int AvgTemp { get; }

        int FeelTemp { get; }

        double SnowFall { get; }

        double RainFall { get; }

        string DayTime { get; }

        string Date { get; }

        string WeekDay { get; }

        string SunsetTime { get; }

        string SunriseTime { get; }

        string Country { get; }

        string Longitude { get; }

        string Latitude { get; }

        int Pressure { get; }

        int Humidity { get; }

        int WindSpeed { get; }

        int WindAngle { get; }

        string Description { get; }

        string DescriptionId { get; }

        string Icon { get; }
    }
}
