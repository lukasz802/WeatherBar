using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using WebApi.Model.DataTransferObjects;

namespace WebApi.Model.Converters
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
            return typeof(FourDaysForecastTransferObject).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                JObject item = JObject.Load(reader);

                return new FourDaysForecastTransferObject()
                {
                    HourlyData = PrepareHourlyForecastData(item),
                    DailyData = PrepareDailyForecastData(item),
                };
            }
            catch
            {
                throw new JsonException();
            }
        }

        #endregion

        #region Methods

        private List<HourlyForecastTransferObject> PrepareHourlyForecastData(JObject weatherForecastData)
        {
            var result = new List<HourlyForecastTransferObject>();

            foreach (var item in ((JArray)weatherForecastData["list"]).Take(40))
            {
                result.Add(new HourlyForecastTransferObject()
                {
                    Description = ((JArray)item["weather"])[0]["description"].ToObject<string>(),
                    FeelTemp = item["main"]["feels_like"].ToObject<double>(),
                    AvgTemp = item["main"]["temp"].ToObject<double>(),
                    Pressure = item["main"]["pressure"].ToObject<int>(),
                    Humidity = item["main"]["humidity"].ToObject<int>(),
                    RainFall = item["rain"] != null ? item["rain"].FirstOrDefault(x => x.Path.Contains("3h")).ToObject<double>() : 0,
                    SnowFall = item["snow"] != null ? item["snow"].FirstOrDefault(x => x.Path.Contains("3h")).ToObject<double>() : 0,
                    WindAngle = item["wind"]["deg"].ToObject<int>(),
                    WindSpeed = item["wind"]["speed"].ToObject<double>(),
                    Icon = ((JArray)item["weather"])[0]["icon"].ToObject<string>(),
                    DescriptionId = ((JArray)item["weather"])[0]["id"].ToObject<string>(),
                    Date = item["dt_txt"].ToObject<DateTime>(),
                    DayTime = item["dt"].ToObject<long>()
                });
            }

            return result;
        }

        private List<DailyForecastTransferObject> PrepareDailyForecastData(JObject weatherForecastData)
        {
            var result = new List<DailyForecastTransferObject>();
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
                IGrouping<string, JToken> groupingElement = (from value in item.Values
                                                             group value by Regex.Match(((JArray)value["weather"])[0]["icon"].ToObject<string>(), @"\d+").Value into t
                                                             orderby t.Count() descending
                                                             select t).FirstOrDefault();

                result.Add(new DailyForecastTransferObject()
                {
                    MaxTemp = (from value in item.Values
                               select value["main"]["temp"].ToObject<double>()).Max(),
                    MinTemp = (from value in item.Values
                               select value["main"]["temp"].ToObject<double>()).Min(),
                    Icon = groupingElement.Key + "d",
                    WeekDay = DateTime.Now.AddDays(counter).DayOfWeek,
                    Description = ((JArray)groupingElement.FirstOrDefault()["weather"])[0]["description"].ToObject<string>(),
                    Date = item.Keyword,
                    DescriptionId = ((JArray)groupingElement.FirstOrDefault()["weather"])[0]["id"].ToObject<string>()
                });

                counter++;
            }

            return result;
        }

        #endregion
    }
}
