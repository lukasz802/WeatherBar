using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using WeatherBar.WebApi.Models;

namespace WeatherBar.Utils
{
    #region Public methods

    public static class ViewModelUtils
    {
        public static IEnumerable<DailyForecast> PrepareFourDaysForecastData(WeatherForecastData weatherForecastData)
        {
            var result = new List<DailyForecast>();

            if (weatherForecastData == null)
            {
                return result;
            }

            var tempList = weatherForecastData.Forecast.Where(x => !x.DtTxt.Contains(DateTime.Now.ToString("yyyy-MM-dd")))
                                                       .GroupBy(x => Regex.Match(x.DtTxt, @"\d{4}-\d{1,2}-\d{1,2}").Value)
                                                       .Select(x => new
                                                       {
                                                           Keyword = x.Key,
                                                           Values = x.Select(v => v)
                                                       });

            int counter = 1;

            foreach (var listElement in tempList.Take(4))
            {
                var weekDay = DateTime.Now.AddDays(counter).DayOfWeek;
                Match local = Regex.Match(listElement.Keyword, @"(?<Month>(\d{1,2}))-(?<Day>(\d{1,2}))", RegexOptions.RightToLeft);
                var groupingElement = (from value in listElement.Values
                                       group value by Regex.Match(value.WeatherData[0].Icon, @"\d+").Value into t
                                       orderby t.Count() descending
                                       select t).FirstOrDefault();

                result.Add(new DailyForecast()
                {
                    Icon = groupingElement.Key + "d",
                    MaxTemp = Convert.ToInt32((from value in listElement.Values
                                               orderby value.Main.Temp descending
                                               select value.Main.Temp).FirstOrDefault()),
                    MinTemp = Convert.ToInt32((from value in listElement.Values
                                               orderby value.Main.Temp ascending
                                               select value.Main.Temp).FirstOrDefault()),
                    Date = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(weekDay) + ", " +
                           int.Parse(local.Groups["Day"].Value).ToString() + " " + DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(int.Parse(local.Groups["Month"].Value)),
                    Description = (DateTimeFormatInfo.CurrentInfo.GetDayName(weekDay).First().ToString().ToUpper() +
                                   DateTimeFormatInfo.CurrentInfo.GetDayName(weekDay).Substring(1)) + ", " +
                                   groupingElement.FirstOrDefault().WeatherData[0].Description,
                });

                counter++;
            }

            return result;
        }

        public static HourlyForecast PrepareCurrentWeatherData(CurrentWeatherData currentWeatherData)
        {
            return new HourlyForecast()
            {
                AvgTemp = Convert.ToInt32(currentWeatherData.MainData.Temp),
                FeelTemp = Convert.ToInt32(currentWeatherData.MainData.FeelsLike),
                Pressure = Convert.ToInt32(currentWeatherData.MainData.Pressure),
                Humidity = Convert.ToInt32(currentWeatherData.MainData.Humidity),
                RainFall = Math.Round(Convert.ToDouble(currentWeatherData.RainData._1h), 2),
                SnowFall = Math.Round(Convert.ToDouble(currentWeatherData.SnowData._1h), 2),
                WindAngle = Convert.ToInt32(currentWeatherData.WindData.Deg - 180),
                WindSpeed = Convert.ToInt32(currentWeatherData.WindData.Speed * 3.6),
                Description =
                    currentWeatherData.WeatherData[0].Description.FirstOrDefault().ToString().ToUpper() + currentWeatherData.WeatherData[0].Description.Substring(1),
                Icon = currentWeatherData.WeatherData[0].Icon,
                SunsetTime = (SharedFunctions.UnixTimeStampToDateTime(currentWeatherData.SysData.Sunset) + DateTimeOffset.Now.Offset).ToString("HH:mm"),
                SunriseTime = (SharedFunctions.UnixTimeStampToDateTime(currentWeatherData.SysData.Sunrise) + DateTimeOffset.Now.Offset).ToString("HH:mm"),
                DayTime = "Teraz",
                Country = currentWeatherData.SysData.Country,
                CityName = currentWeatherData.Name,
                Longitude = SharedFunctions.ConvertCoordinatesFromDecToDeg(currentWeatherData.CoordData.Lon, true),
                Latitude = SharedFunctions.ConvertCoordinatesFromDecToDeg(currentWeatherData.CoordData.Lat, false),
            };
        }

        public static IEnumerable<HourlyForecast> PrepareHourlyForecastData(CurrentWeatherData currentWeatherData, WeatherForecastData weatherForecastData)
        {
            var result = new List<HourlyForecast>();

            if (weatherForecastData == null)
            {
                return result;
            }

            foreach (var listElement in weatherForecastData.Forecast.Take(5))
            {
                result.Add(new HourlyForecast()
                {
                    AvgTemp = Convert.ToInt32(listElement.Main.Temp),
                    FeelTemp = Convert.ToInt32(listElement.Main.FeelsLike),
                    Pressure = Convert.ToInt32(listElement.Main.Pressure),
                    Humidity = Convert.ToInt32(listElement.Main.Humidity),
                    RainFall = Math.Round(Convert.ToDouble(listElement.Rain._3h), 2),
                    SnowFall = Math.Round(Convert.ToDouble(listElement.Snow._3h), 2),
                    WindAngle = Convert.ToInt32(listElement.Wind.Deg - 180),
                    WindSpeed = Convert.ToInt32(listElement.Wind.Speed * 3.6),
                    Description =
                        listElement.WeatherData[0].Description.FirstOrDefault().ToString().ToUpper() + listElement.WeatherData[0].Description.Substring(1),
                    Icon = listElement.WeatherData[0].Icon,
                    SunsetTime = (SharedFunctions.UnixTimeStampToDateTime(currentWeatherData.SysData.Sunset) + DateTimeOffset.Now.Offset).ToString("HH:mm"),
                    SunriseTime = (SharedFunctions.UnixTimeStampToDateTime(currentWeatherData.SysData.Sunrise) + DateTimeOffset.Now.Offset).ToString("HH:mm"),
                    DayTime = (SharedFunctions.UnixTimeStampToDateTime(listElement.Dt) + DateTimeOffset.Now.Offset).ToString("HH:mm"),
                    Country = currentWeatherData.SysData.Country,
                    CityName = currentWeatherData.Name,
                    Longitude = SharedFunctions.ConvertCoordinatesFromDecToDeg(currentWeatherData.CoordData.Lon, true),
                    Latitude = SharedFunctions.ConvertCoordinatesFromDecToDeg(currentWeatherData.CoordData.Lat, false),
                });
            }

            return result;
        }

        #endregion

        #region Structs

        public struct DailyForecast
        {
            public int MaxTemp { get; set; }

            public int MinTemp { get; set; }

            public string Date { get; set; }

            public string Icon { get; set; }

            public string Description { get; set; }
        }

        public struct HourlyForecast
        {
            public string Description { get; set; }

            public string CityName { get; set; }

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
        }

        #endregion

    }
}
