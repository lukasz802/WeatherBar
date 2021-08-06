using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using WeatherBar.WebApi.Models.Factories;
using WeatherBar.WebApi.Models.Interfaces;

namespace WeatherBar.WebApi.Models.Converters
{
    internal class FourDaysForecastDataConverter : JsonConverter
    {
        #region Public methods

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IFourDaysData).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                JObject item = JObject.Load(reader);
                IFourDaysData transferObject = WeatherDataFactory.GetFourDaysDataTransferObject();

                transferObject.HourlyData = PrepareHourlyForecastData(item);
                transferObject.DailyData = PrepareDailyForecastData(item);

                return transferObject;
            }
            catch
            {
                throw new JsonException();
            }
        }

        #endregion

        #region Methods

        private IEnumerable<IHourlyData> PrepareHourlyForecastData(JObject weatherForecastData)
        {
            var result = new List<IHourlyData>();

            foreach (var item in ((JArray)weatherForecastData["list"]).Take(40))
            {
                IHourlyData transferObject = WeatherDataFactory.GetHourlyDataTransferObject();
                DateTime localDate = item["dt_txt"].ToObject<DateTime>();

                transferObject.AvgTemp = Convert.ToInt32(item["main"]["temp"].ToObject<double>());
                transferObject.FeelTemp = Convert.ToInt32(item["main"]["feels_like"].ToObject<double>());
                transferObject.Pressure = Convert.ToInt32(item["main"]["pressure"].ToObject<int>());
                transferObject.Humidity = Convert.ToInt32(item["main"]["humidity"].ToObject<int>());
                transferObject.RainFall = Math.Round(Convert.ToDouble(item["rain"]?.Where(x => x.Path.Contains("3h")).FirstOrDefault().ToObject<double>()), 1);
                transferObject.SnowFall = Math.Round(Convert.ToDouble(item["snow"]?.Where(x => x.Path.Contains("3h")).FirstOrDefault().ToObject<double>()), 1);
                transferObject.WindAngle = Convert.ToInt32(item["wind"]["deg"].ToObject<int>() - 180);
                transferObject.WindSpeed = Convert.ToInt32(item["wind"]["speed"].ToObject<double>() * 3.6);
                transferObject.Description =
                        ((JArray)item["weather"])[0]["description"].ToObject<string>().FirstOrDefault().ToString().ToUpper() + ((JArray)item["weather"])[0]["description"].ToObject<string>().Substring(1);
                transferObject.Icon = ((JArray)item["weather"])[0]["icon"].ToObject<string>();
                transferObject.DayTime = (Utils.UnixTimeStampToDateTime(item["dt"].ToObject<long>()) + DateTimeOffset.Now.Offset).ToString("HH:mm");
                transferObject.Date = localDate.ToString("dd MMMM").First() == '0' ? localDate.ToString("dd MMMM").Remove(0,1) : localDate.ToString("dd MMMM");
                transferObject.WeekDay = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.GetDayName(localDate.DayOfWeek).FirstOrDefault().ToString().ToUpper() +
                    Thread.CurrentThread.CurrentUICulture.DateTimeFormat.GetDayName(localDate.DayOfWeek).Substring(1);

                result.Add(transferObject);
            }

            return result;
        }

        private IEnumerable<IDailyData> PrepareDailyForecastData(JObject weatherForecastData)
        {
            var result = new List<IDailyData>();
            var tempList = ((JArray)weatherForecastData["list"]).Where(x => !x["dt_txt"].ToObject<string>().Contains(DateTime.Now.ToString("yyyy-MM-dd")))
                                                                .GroupBy(x => Regex.Match(x["dt_txt"].ToObject<string>(), @"\d{4}-\d{1,2}-\d{1,2}").Value)
                                                                .Select(x => new
                                                                {
                                                                    Keyword = x.Key,
                                                                    Values = x.Select(v => v)
                                                                });

            int counter = 1;

            foreach (var item in tempList.Take(4))
            {
                IDailyData transferObject = WeatherDataFactory.GetDailyDataTransferObject();
                var weekDay = DateTime.Now.AddDays(counter).DayOfWeek;
                Match local = Regex.Match(item.Keyword, @"(?<Month>(\d{1,2}))-(?<Day>(\d{1,2}))", RegexOptions.RightToLeft);
                var groupingElement = (from value in item.Values
                                       group value by Regex.Match(((JArray)value["weather"])[0]["icon"].ToObject<string>(), @"\d+").Value into t
                                       orderby t.Count() descending
                                       select t).FirstOrDefault();

                transferObject.Icon = groupingElement.Key + "d";
                transferObject.MaxTemp = Convert.ToInt32((from value in item.Values
                                                          select value["main"]["temp"].ToObject<double>()).Max());
                transferObject.MinTemp = Convert.ToInt32((from value in item.Values
                                                          select value["main"]["temp"].ToObject<double>()).Min());
                transferObject.Date = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(weekDay) + ", " +
                           int.Parse(local.Groups["Day"].Value).ToString() + " " + DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(int.Parse(local.Groups["Month"].Value));
                transferObject.Description = DateTimeFormatInfo.CurrentInfo.GetDayName(weekDay).First().ToString().ToUpper() +
                           DateTimeFormatInfo.CurrentInfo.GetDayName(weekDay).Substring(1) + ", " +
                           ((JArray)groupingElement.FirstOrDefault()["weather"])[0]["description"].ToObject<string>();

                result.Add(transferObject);
                counter++;
            }

            return result;
        }

        #endregion
    }
}
