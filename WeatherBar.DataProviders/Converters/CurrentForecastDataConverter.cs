using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;
using WeatherBar.Model;
using WeatherBar.Utils.Extensions;

namespace WeatherBar.DataProviders.Converters
{
    internal class CurrentForecastDataConverter : JsonConverter
    {
        #region Public methods

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(HourlyForecast).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                JObject item = JObject.Load(reader);

                return new HourlyForecast(
                    description: ((JArray)item["weather"])[0]["description"].ToObject<string>(),
                    descriptionId: ((JArray)item["weather"])[0]["id"].ToObject<string>(),
                    cityId : item["id"].ToObject<string>(),
                    cityName: item["name"].ToObject<string>(),
                    snowFall: Math.Round(item["snow"] != null ? item["snow"].FirstOrDefault(x => x.Path.Contains("1h")).ToObject<double>() : 0, 1),
                    rainFall: Math.Round(item["rain"] != null ? item["rain"].FirstOrDefault(x => x.Path.Contains("1h")).ToObject<double>() : 0, 1),
                    sunriseTime: (item["sys"]["sunrise"].ToObject<long>().ToDateTime() + DateTimeOffset.Now.Offset).ToString("HH:mm"),
                    sunsetTime: (item["sys"]["sunset"].ToObject<long>().ToDateTime() + DateTimeOffset.Now.Offset).ToString("HH:mm"),
                    country: item["sys"]["country"].ToObject<string>(),
                    longitude: item["coord"]["lon"].ToObject<double>(),
                    latitude: item["coord"]["lat"].ToObject<double>(),
                    pressure: item["main"]["pressure"].ToObject<int>(),
                    humidity: item["main"]["humidity"].ToObject<int>(),
                    avgTemp: item["main"]["temp"].ToObject<int>(),
                    feelTemp: item["main"]["feels_like"].ToObject<int>(),
                    windAngle: item["wind"]["deg"].ToObject<int>() - 180,
                    windSpeed: Convert.ToInt32(item["wind"]["speed"].ToObject<double>() * 3.6),
                    icon: ((JArray)item["weather"])[0]["icon"].ToObject<string>());
            }
            catch
            {
                throw new JsonException();
            }
        }

        #endregion
    }
}
