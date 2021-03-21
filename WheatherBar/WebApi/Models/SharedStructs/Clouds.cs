using Newtonsoft.Json;

namespace WeatherBar.WebApi.Models.SharedStructs
{
    public class Clouds
    {
        [JsonProperty("all")]
        public int All { get; set; }
    }
}
