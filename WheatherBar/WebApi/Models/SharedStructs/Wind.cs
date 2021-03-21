using Newtonsoft.Json;

namespace WeatherBar.WebApi.Models.SharedStructs
{
    public class Wind
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }

        [JsonProperty("deg")]
        public int Deg { get; set; }
    }
}
