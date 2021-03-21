using Newtonsoft.Json;
using System.Collections.Generic;
using WeatherBar.WebApi.Models.Interfaces;
using WeatherBar.WebApi.Models.SharedStructs;

namespace WeatherBar.WebApi.Models
{
    public class CurrentWeatherData : IWeatherData
    {
        #region Properties

        [JsonProperty("coord")]
        public Coord CoordData { get; private set; }

        [JsonProperty("weather")]
        public List<Weather> WheatherData { get; }

        [JsonProperty("main")]
        public Main MainData { get; set; }

        [JsonProperty("wind")]
        public Wind WindData { get; set; }

        [JsonProperty("clouds")]
        public Clouds CloudsData { get; set; }

        [JsonProperty("snow")]
        public Snow SnowData { get; set; }

        [JsonProperty("rain")]
        public Rain RainData { get; set; }

        [JsonProperty("sys")]
        public Sys SysData { get; set; }

        [JsonProperty("base")]
        public string Base { get; set; }

        [JsonProperty("visibility")]
        public int Visibility { get; set; }

        [JsonProperty("dt")]
        public int Dt { get; set; }

        [JsonProperty("timezone")]
        public int Timezone { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("cod")]
        public int Cod { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the CurrentWeatherData class.
        /// </summary>
        public CurrentWeatherData(List<Weather> wheatherData = default(List<Weather>), Coord coord = default(Coord), Main main = default(Main), Wind wind = default(Wind),
            Clouds clouds = default(Clouds), Snow snow = default(Snow), Rain rain = default(Rain), Sys sys = default(Sys), string _base = default(string), 
            int visisbility = default(int), int dt = default(int), int timezone = default(int), int id = default(int), string name = default(string), int cod = default(int))
        {
            CoordData = coord;
            WheatherData = wheatherData;
            MainData = main;
            WindData = wind;
            CloudsData = clouds;
            SnowData = snow;
            RainData = rain;
            SysData = sys;
            Base = _base;
            Visibility = visisbility;
            Dt = dt;
            Timezone = timezone;
            Id = id;
            Name = name;
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

            [JsonProperty("humidity")]
            public int Humidity { get; set; }
        }

        public struct Sys
        {
            [JsonProperty("type")]
            public int Type { get; set; }

            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("country")]
            public string Country { get; set; }

            [JsonProperty("sunrise")]
            public int Sunrise { get; set; }

            [JsonProperty("sunset")]
            public int Sunset { get; set; }
        }

        #endregion
    }
}
