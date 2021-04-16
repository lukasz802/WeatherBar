using Newtonsoft.Json;
using System.Collections.Generic;
using WeatherBar.WebApi.Models.Interfaces;
using WeatherBar.WebApi.Models.SharedStructs;

namespace WeatherBar.WebApi.Models
{
    public class WeatherForecastData : IWeatherData
    {
        #region Properties

        [JsonProperty("cod")]
        public string Cod { get; set; }

        [JsonProperty("message")]
        public int Message { get; set; }

        [JsonProperty("cnt")]
        public int Cnt { get; set; }

        [JsonProperty("list")]
        public List<List> Forecast { get; }

        [JsonProperty("city")]
        public City CityData { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the WeatherForecastData class.
        /// </summary>
        public WeatherForecastData(List<List> forecast = default(List<List>), City cityData = default(City), int cnt = default(int),
            int message = default(int), string cod = default(string))
        {
            CityData = cityData;
            Forecast = forecast;
            Cnt = cnt;
            Message = message;
            Cod = cod;
        }

        #endregion

        #region Structs

        public struct Main
        {
            [JsonProperty("temp")]
            public double Temp { get; set; }

            [JsonProperty("feels_like")]
            public double FeelsLike { get; set; }

            [JsonProperty("temp_min")]
            public double TempMin { get; set; }

            [JsonProperty("temp_max")]
            public double TempMax { get; set; }

            [JsonProperty("pressure")]
            public int Pressure { get; set; }

            [JsonProperty("sea_level")]
            public int SeaLevel { get; set; }

            [JsonProperty("grnd_level")]
            public int GrndLevel { get; set; }

            [JsonProperty("humidity")]
            public int Humidity { get; set; }

            [JsonProperty("temp_kf")]
            public double TempKf { get; set; }
        }

        public struct Sys
        {
            [JsonProperty("pod")]
            public string Pod { get; set; }
        }

        public struct List
        {
            [JsonProperty("dt")]
            public int Dt { get; set; }

            [JsonProperty("main")]
            public Main Main { get; set; }

            [JsonProperty("weather")]
            public List<Weather> WeatherData { get; set; }

            [JsonProperty("clouds")]
            public Clouds Clouds { get; set; }

            [JsonProperty("wind")]
            public Wind Wind { get; set; }

            [JsonProperty("visibility")]
            public int Visibility { get; set; }

            [JsonProperty("pop")]
            public double Pop { get; set; }

            [JsonProperty("sys")]
            public Sys Sys { get; set; }

            [JsonProperty("dt_txt")]
            public string DtTxt { get; set; }

            [JsonProperty("snow")]
            public Snow Snow { get; set; }

            [JsonProperty("rain")]
            public Rain Rain { get; set; }
        }

        public struct City
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("coord")]
            public Coord Coord { get; set; }

            [JsonProperty("country")]
            public string Country { get; set; }

            [JsonProperty("population")]
            public int Population { get; set; }

            [JsonProperty("timezone")]
            public int Timezone { get; set; }

            [JsonProperty("sunrise")]
            public int Sunrise { get; set; }

            [JsonProperty("sunset")]
            public int Sunset { get; set; }
        }

        #endregion
    }
}
