using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using WeatherBar.Model;

namespace WeatherBar.DataProviders.Converters
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
            return typeof(FourDaysForecast).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                JObject item = JObject.Load(reader);

                return new FourDaysForecast(
                    hourlyData: PrepareHourlyForecastData(item),
                    dailyData: PrepareDailyForecastData(item));
            }
            catch
            {
                throw new JsonException();
            }
        }

        #endregion

        #region Methods

        private List<HourlyForecast> PrepareHourlyForecastData(JObject weatherForecastData)
        {
            var result = new List<HourlyForecast>();

            foreach (var item in ((JArray)weatherForecastData["list"]).Take(40))
            {
                result.Add(new HourlyForecast(
                    description: ((JArray)item["weather"])[0]["description"].ToObject<string>(),
                    feelTemp: item["main"]["feels_like"].ToObject<double>(),
                    avgTemp: item["main"]["temp"].ToObject<double>(),
                    pressure: item["main"]["pressure"].ToObject<int>(),
                    humidity: item["main"]["humidity"].ToObject<int>(),
                    rainFall: item["rain"] != null ? item["rain"].FirstOrDefault(x => x.Path.Contains("3h")).ToObject<double>() : 0,
                    snowFall: item["snow"] != null ? item["snow"].FirstOrDefault(x => x.Path.Contains("3h")).ToObject<double>() : 0,
                    windAngle: item["wind"]["deg"].ToObject<int>(),
                    windSpeed: item["wind"]["speed"].ToObject<double>(),
                    icon: ((JArray)item["weather"])[0]["icon"].ToObject<string>(),
                    descriptionId: ((JArray)item["weather"])[0]["id"].ToObject<string>(),
                    date: item["dt_txt"].ToObject<DateTime>(),
                    dayTime: item["dt"].ToObject<long>()));
            }

            return result;
        }

        private List<DailyForecast> PrepareDailyForecastData(JObject weatherForecastData)
        {
            var result = new List<DailyForecast>();
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

                result.Add(new DailyForecast(
                    maxTemp: (from value in item.Values
                               select value["main"]["temp"].ToObject<double>()).Max(),
                    minTemp: (from value in item.Values
                               select value["main"]["temp"].ToObject<double>()).Min(),
                    icon: groupingElement.Key + "d",
                    weekDay: DateTime.Now.AddDays(counter).DayOfWeek,
                    description: ((JArray)groupingElement.FirstOrDefault()["weather"])[0]["description"].ToObject<string>(),
                    date: item.Keyword,
                    descriptionId: ((JArray)groupingElement.FirstOrDefault()["weather"])[0]["id"].ToObject<string>()));

                counter++;
            }

            return result;
        }

        #endregion
    }
}
