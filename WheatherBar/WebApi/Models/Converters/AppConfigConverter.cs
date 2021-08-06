using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace WeatherBar.WebApi.Models.Converters
{
    internal class AppConfigConvrter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(string).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                JObject item = JObject.Load(reader);
                var tokenArray = (JArray)item["ApiKeys"];

                return tokenArray[new Random().Next(0, tokenArray.Count)].ToObject<string>();
            }
            catch
            {
                throw new JsonException();
            }
        }
    }
}
