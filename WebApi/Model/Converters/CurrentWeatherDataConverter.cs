using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;
using WebApi.Model.Factories;
using WebApi.Model.Interfaces;

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
            return typeof(IHourlyData).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                JObject item = JObject.Load(reader);
                return WeatherDataFactory.GetHourlyDataTransferObject(
                    description: ((JArray)item["weather"])[0]["description"].ToObject<string>(),
                    descriptionId: ((JArray)item["weather"])[0]["id"].ToObject<string>(),
                    cityId: item["id"].ToObject<string>(),
                    cityName: item["name"].ToObject<string>(),
                    snowFall: item["snow"] != null ? item["snow"].Where(x => x.Path.Contains("1h")).FirstOrDefault().ToObject<double>() : 0,
                    rainFall: item["rain"] != null ? item["rain"].Where(x => x.Path.Contains("1h")).FirstOrDefault().ToObject<double>() : 0,
                    sunriseTime: item["sys"]["sunrise"].ToObject<int>(),
                    sunsetTime: item["sys"]["sunset"].ToObject<int>(),
                    country: item["sys"]["country"].ToObject<string>(),
                    longitude: item["coord"]["lon"].ToObject<double>(),
                    latitude: item["coord"]["lat"].ToObject<double>(),
                    pressure: item["main"]["pressure"].ToObject<int>(),
                    humidity: item["main"]["humidity"].ToObject<int>(),
                    avgTemp: item["main"]["temp"].ToObject<double>(),
                    feelTemp: item["main"]["feels_like"].ToObject<double>(),
                    windAngle: item["wind"]["deg"].ToObject<int>(),
                    windSpeed: item["wind"]["speed"].ToObject<double>(),
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
