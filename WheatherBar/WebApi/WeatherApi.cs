using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherBar.WebApi.Models;
using WeatherBar.WebApi.Models.Enums;
using WeatherBar.WebApi.Models.Interfaces;

namespace WeatherBar.WebApi
{
    public class WeatherApi : ServiceClient<WeatherApi>, IWeatherApi
    {

        #region Properties

        public string ApiKey { get; }

        public Units Units { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MyAPI class.
        /// </summary>
        public WeatherApi(string apiKey, Units units = Units.Standard) : base()
        {
            HttpClient.Timeout = TimeSpan.FromSeconds(5);
            this.ApiKey = apiKey;
            this.Units = units;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get current weather data.
        /// </summary>
        public CurrentWeatherData GetCurrentWeatherData(string cityName)
        {
            return Task.Factory.StartNew(() => GetCurrentWeatherDataBodyAsync(cityName)).Unwrap().GetAwaiter().GetResult();
        }

        public WeatherForecastData GetWeatherForecastData(string cityName)
        {
            return Task.Factory.StartNew(() => GetWeatherForecastDataBodyAsync(cityName)).Unwrap().GetAwaiter().GetResult();
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
                            return weatherDataType == WeatherDataType.CurrentWeather ? SafeJsonConvert.DeserializeObject<CurrentWeatherData>(responseContent)
                                : (IWeatherData)SafeJsonConvert.DeserializeObject<WeatherForecastData>(responseContent);
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

        private async Task<CurrentWeatherData> GetCurrentWeatherDataBodyAsync(string cityName)
        {
            return (CurrentWeatherData) await GetForecastDataAsync(WeatherDataType.CurrentWeather, cityName);
        }

        private async Task<WeatherForecastData> GetWeatherForecastDataBodyAsync(string cityName)
        {
            return (WeatherForecastData) await GetForecastDataAsync(WeatherDataType.WeatherForecast, cityName);
        }

        #endregion
    }
}
