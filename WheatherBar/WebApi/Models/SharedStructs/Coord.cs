using Newtonsoft.Json;

namespace WeatherBar.WebApi.Models.SharedStructs
{
    public struct Coord
    {
        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }
    }
}
