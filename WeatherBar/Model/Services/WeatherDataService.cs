using WebApi.Model.DataTransferObjects;
using WeatherBar.Model.Interfaces;
using System.Linq;
using WebApi.Model.Enums;
using WeatherBar.Model.Services.Interfaces;

namespace WeatherBar.Model.Services
{
    public class WeatherDataService : IWeatherDataService
    {
        #region Public methods

        public IHourlyData GetHourlyData(CallType callType, string cityData)
        {
            HourlyForecastTransferObject hourlyForecastTransferObject;

            if (callType == CallType.ByCityName)
            {
                hourlyForecastTransferObject = App.ApiClient.GetCurrentWeatherDataByCityName(cityData);
            }
            else
            {
                hourlyForecastTransferObject = App.ApiClient.GetCurrentWeatherDataByCityId(cityData);
            }

            return ParseHourlyForecastTransferObject(hourlyForecastTransferObject);
        }

        public IHourlyData GetEmptyHourlyData()
        {
            return HourlyForecast.Empty();
        }

        public IFourDaysData GetFourDaysData(CallType callType, string cityData)
        {
            FourDaysForecastTransferObject fourDaysForecastTransferObject;

            if (callType == CallType.ByCityName)
            {
                fourDaysForecastTransferObject = App.ApiClient.GetFourDaysForecastDataByCityName(cityData);
            }
            else
            {
                fourDaysForecastTransferObject = App.ApiClient.GetFourDaysForecastDataByCityId(cityData);
            }

            return new FourDaysForecast(fourDaysForecastTransferObject.HourlyData.Select(
                x => ParseHourlyForecastTransferObject(x)).ToList(), fourDaysForecastTransferObject.DailyData.Select(x => ParseDailyForecastTransferObject(x)).ToList());
        }

        #endregion

        #region Private methods

        private HourlyForecast ParseHourlyForecastTransferObject(HourlyForecastTransferObject hourlyForecastTransferObject)
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

        private DailyForecast ParseDailyForecastTransferObject(DailyForecastTransferObject dailyForecastTransferObject)
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
