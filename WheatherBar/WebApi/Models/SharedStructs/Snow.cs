using Newtonsoft.Json;

namespace WeatherBar.WebApi.Models.SharedStructs
{
    public struct Snow
    {
        [JsonProperty("3h")]
        public double _3h { get; set; }

        [JsonProperty("1h")]
        public double _1h { get; set; }
    }
}
