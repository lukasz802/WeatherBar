using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;
using WebApi.Models.Factories;
using WebApi.Models.Interfaces;

namespace WebApi.Models.Converters
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
                IHourlyData transferObject = WeatherDataFactory.GetHourlyDataTransferObject();

                transferObject.AvgTemp = Convert.ToInt32(item["main"]["temp"].ToObject<double>());
                transferObject.FeelTemp = Convert.ToInt32(item["main"]["feels_like"].ToObject<double>());
                transferObject.Pressure = Convert.ToInt32(item["main"]["pressure"].ToObject<int>());
                transferObject.Humidity = Convert.ToInt32(item["main"]["humidity"].ToObject<int>());
                transferObject.RainFall = Math.Round(Convert.ToDouble(item["rain"]?.Where(x => x.Path.Contains("1h")).FirstOrDefault().ToObject<double>()), 1);
                transferObject.SnowFall = Math.Round(Convert.ToDouble(item["snow"]?.Where(x => x.Path.Contains("1h")).FirstOrDefault().ToObject<double>()), 1);
                transferObject.WindAngle = Convert.ToInt32(item["wind"]["deg"].ToObject<int>() - 180);
                transferObject.WindSpeed = Convert.ToInt32(item["wind"]["speed"].ToObject<double>() * 3.6);
                transferObject.Description =
                        ((JArray)item["weather"])[0]["description"].ToObject<string>().FirstOrDefault().ToString().ToUpper() + ((JArray)item["weather"])[0]["description"].ToObject<string>().Substring(1);
                transferObject.Icon = ((JArray)item["weather"])[0]["icon"].ToObject<string>();
                transferObject.SunsetTime = (Utils.UnixTimeStampToDateTime(item["sys"]["sunset"].ToObject<int>()) + DateTimeOffset.Now.Offset).ToString("HH:mm");
                transferObject.SunriseTime = (Utils.UnixTimeStampToDateTime(item["sys"]["sunrise"].ToObject<int>()) + DateTimeOffset.Now.Offset).ToString("HH:mm");
                transferObject.DayTime = "Teraz";
                transferObject.CityId = item["id"].ToObject<int>();
                transferObject.CityName = item["name"].ToObject<string>();
                transferObject.Country = item["sys"]["country"].ToObject<string>();
                transferObject.Longitude = Utils.ConvertCoordinatesFromDecToDeg(item["coord"]["lon"].ToObject<double>(), true);
                transferObject.Latitude = Utils.ConvertCoordinatesFromDecToDeg(item["coord"]["lat"].ToObject<double>(), false);

                return transferObject;
            }
            catch
            {
                throw new JsonException();
            }
        }

        #endregion
    }
}
