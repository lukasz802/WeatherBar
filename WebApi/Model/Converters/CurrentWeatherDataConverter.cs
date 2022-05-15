using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;
using WebApi.Model.DataTransferObjects;

namespace WebApi.Model.Converters
{
    internal class CurrentWeatherDataConverter : JsonConverter
    {
        #region Public methods

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(HourlyForecastTransferObject).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                JObject item = JObject.Load(reader);

                return new HourlyForecastTransferObject
                {
                    Description = ((JArray)item["weather"])[0]["description"].ToObject<string>(),
                    DescriptionId = ((JArray)item["weather"])[0]["id"].ToObject<string>(),
                    CityId = item["id"].ToObject<string>(),
                    CityName = item["name"].ToObject<string>(),
                    SnowFall = item["snow"] != null ? item["snow"].FirstOrDefault(x => x.Path.Contains("1h")).ToObject<double>() : 0,
                    RainFall = item["rain"] != null ? item["rain"].FirstOrDefault(x => x.Path.Contains("1h")).ToObject<double>() : 0,
                    SunriseTime = item["sys"]["sunrise"].ToObject<int>(),
                    SunsetTime = item["sys"]["sunset"].ToObject<int>(),
                    Country = item["sys"]["country"].ToObject<string>(),
                    Longitude = item["coord"]["lon"].ToObject<double>(),
                    Latitude = item["coord"]["lat"].ToObject<double>(),
                    Pressure = item["main"]["pressure"].ToObject<int>(),
                    Humidity = item["main"]["humidity"].ToObject<int>(),
                    AvgTemp = item["main"]["temp"].ToObject<double>(),
                    FeelTemp = item["main"]["feels_like"].ToObject<double>(),
                    WindAngle = item["wind"]["deg"].ToObject<int>(),
                    WindSpeed = item["wind"]["speed"].ToObject<double>(),
                    Icon = ((JArray)item["weather"])[0]["icon"].ToObject<string>()
                };
            }
            catch
            {
                throw new JsonException();
            }
        }

        #endregion
    }
}
