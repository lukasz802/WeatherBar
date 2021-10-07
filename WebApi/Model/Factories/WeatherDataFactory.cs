using WebApi.Model.Interfaces;
using WebApi.Model.DataTransferObjects;
using System;
using System.Collections.Generic;

namespace WebApi.Model.Factories
{
    public static class WeatherDataFactory
    {
        #region Public methods

        public static IHourlyData GetHourlyDataTransferObject(string description, double avgTemp, double feelTemp, double snowFall, double rainFall, int pressure, int humidity, double windSpeed,
            int windAngle, string icon, string country = null, string cityName = null, string cityId = null, long? sunsetTime = null, long? sunriseTime = null,
            double? longitude = null, double? latitude = null, string descriptionId = null, DateTime? date = null, long? dayTime = null)
        {
            return new HourlyForecastTransferObject(description, avgTemp, feelTemp, snowFall, rainFall, pressure, humidity, windSpeed, windAngle, icon, country, cityName,
                cityId, sunsetTime, sunriseTime, longitude, latitude, descriptionId, date, dayTime);
        }

        public static IHourlyData GetEmptyHourlyDataTransferObject()
        {
            return HourlyForecastTransferObject.Empty();
        }

        public static IDailyData GetDailyDataTransferObject(double maxTemp, double minTemp, string icon, string description, string descriptionId, DayOfWeek weekDay, string date)
        {
            return new DailyForecastTransferObject(maxTemp, minTemp, icon, description, descriptionId, weekDay, date);
        }

        public static IFourDaysData GetFourDaysDataTransferObject(IEnumerable<IHourlyData> hourlyData, IEnumerable<IDailyData> dailyData)
        {
            return new FourDaysForecastTransferObject(hourlyData, dailyData);
        }

        #endregion
    }
}
