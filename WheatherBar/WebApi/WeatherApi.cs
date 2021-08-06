using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherBar.WebApi.Models.Enums;
using WeatherBar.WebApi.Models.Interfaces;
using WeatherBar.WebApi.Models.Converters;
using System.IO;

namespace WeatherBar.WebApi
{
    public class WeatherApi : ServiceClient<WeatherApi>, IWeatherApi
    {
        #region Fields and constants

        private const string ConfigFileName = "config.json";

        #endregion

        #region Properties

        public string ApiKey { get; }

        public Units Units { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MyAPI class.
        /// </summary>
        public WeatherApi(Units units = Units.Standard) : base()
        {
            HttpClient.Timeout = TimeSpan.FromSeconds(5);
            this.ApiKey = GetUniqueApiKey();
            this.Units = units;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get current weather data.
        /// </summary>
        public IHourlyData GetCurrentWeatherData(string cityName)
        {
            return Task.Run(() => GetCurrentWeatherDataBodyAsync(cityName)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get four days forecast data.
        /// </summary>
        public IFourDaysData GetFourDaysForecastData(string cityName)
        {
            return Task.Run(() => GetWeatherForecastDataBodyAsync(cityName)).GetAwaiter().GetResult();
        }

        #endregion

        #region Methods

        private async Task<IWeatherData> GetForecastDataAsync(WeatherDataType weatherDataType, string cityName)
        {
            var querry = weatherDataType == WeatherDataType.CurrentWeather ? "weather" : "forecast";
            var url = new Uri($"http://api.openweathermap.org/data/2.5/{querry}?q={cityName}&units={Units}&appid={ApiKey}&lang=pl").ToString();
            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri(url)
            };

            try
            {
                using (HttpResponseMessage httpResponse = await HttpClient.SendAsync(httpRequest).ConfigureAwait(false))
                {
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        httpRequest.Dispose();
                        throw new HttpOperationException($"Operation returned an invalid status code '{httpResponse.StatusCode}'");
                    }
                    else
                    {
                        string responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                        try
                        {
                            return weatherDataType == WeatherDataType.CurrentWeather ? SafeJsonConvert.DeserializeObject<IHourlyData>(responseContent, new CurrentWeatherDataConverter())
                                : SafeJsonConvert.DeserializeObject<IFourDaysData>(responseContent, new FourDaysForecastDataConverter()) as IWeatherData;
                        }
                        catch (JsonException ex)
                        {
                            httpRequest.Dispose();
                            throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw new TaskCanceledException("Unable to get a HTTP response message.", ex);
            }
        }

        private async Task<IHourlyData> GetCurrentWeatherDataBodyAsync(string cityName)
        {
            return (IHourlyData) await GetForecastDataAsync(WeatherDataType.CurrentWeather, cityName);
        }

        private async Task<IFourDaysData> GetWeatherForecastDataBodyAsync(string cityName)
        {
            return (IFourDaysData) await GetForecastDataAsync(WeatherDataType.WeatherForecast, cityName);
        }

        private string GetUniqueApiKey()
        {
            using (var stream = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), ConfigFileName)))
            {
                return SafeJsonConvert.DeserializeObject<string>(stream.ReadToEnd(), new AppConfigConvrter());
            }
        }

        #endregion
    }
}
