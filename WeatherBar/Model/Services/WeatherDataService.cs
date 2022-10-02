using WeatherDataProvider.Model.DataTransferObjects;
using WeatherBar.Model.Interfaces;
using System.Linq;
using WeatherBar.Model.Services.Interfaces;

namespace WeatherBar.Model.Services
{
    public class WeatherDataService : IWeatherDataService
    {
        #region Public methods

        public IHourlyData GetHourlyData(string cityData)
        {
            HourlyForecastTransferObject hourlyForecastTransferObject;

            hourlyForecastTransferObject = App.WeatherDataProvider.GetCurrentForecast(cityData);

            return ParseHourlyForecastTransferObject(hourlyForecastTransferObject);
        }

        public IFourDaysData GetFourDaysData(string cityData)
        {
            FourDaysForecastTransferObject fourDaysForecastTransferObject;

            fourDaysForecastTransferObject = App.WeatherDataProvider.GetFourDaysForecast(cityData);

            return new FourDaysForecast(fourDaysForecastTransferObject.HourlyData.Select(
                x => ParseHourlyForecastTransferObject(x)).ToList(), fourDaysForecastTransferObject.DailyData.Select(x => ParseDailyForecastTransferObject(x)).ToList());
        }

        #endregion

        #region Private methods

        private IHourlyData ParseHourlyForecastTransferObject(HourlyForecastTransferObject hourlyForecastTransferObject)
        {
            return new HourlyForecast(
                hourlyForecastTransferObject.Description,
                hourlyForecastTransferObject.AvgTemp,
                hourlyForecastTransferObject.FeelTemp,
                hourlyForecastTransferObject.SnowFall,
                hourlyForecastTransferObject.RainFall,
                hourlyForecastTransferObject.Pressure,
                hourlyForecastTransferObject.Humidity,
                hourlyForecastTransferObject.WindSpeed,
                hourlyForecastTransferObject.WindAngle,
                hourlyForecastTransferObject.Icon,
                hourlyForecastTransferObject.Country,
                hourlyForecastTransferObject.CityName,
                hourlyForecastTransferObject.CityId,
                hourlyForecastTransferObject.SunsetTime,
                hourlyForecastTransferObject.SunriseTime,
                hourlyForecastTransferObject.Longitude,
                hourlyForecastTransferObject.Latitude,
                hourlyForecastTransferObject.DescriptionId,
                hourlyForecastTransferObject.Date,
                hourlyForecastTransferObject.DayTime);
        }

        private IDailyData ParseDailyForecastTransferObject(DailyForecastTransferObject dailyForecastTransferObject)
        {
            return new DailyForecast(
                dailyForecastTransferObject.MaxTemp,
                dailyForecastTransferObject.MinTemp,
                dailyForecastTransferObject.Icon,
                dailyForecastTransferObject.Description,
                dailyForecastTransferObject.DescriptionId,
                dailyForecastTransferObject.WeekDay,
                dailyForecastTransferObject.Date);
        }

        #endregion
    }
}
