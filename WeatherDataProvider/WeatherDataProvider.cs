using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherDataProvider.Model.Enums;
using WeatherDataProvider.Model.Interfaces;
using WeatherDataProvider.Model.Converters;
using WeatherDataProvider.Model.DataTransferObjects;
using System.Web;
using WeatherDataProvider.Interfaces;

namespace WeatherDataProvider
{
    public class WeatherDataProvider : IWeatherDataProvider
    {
        #region Properties

        public HttpClient HttpClient { get; private set; }

        public string ApiKey { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the WeatherDataProvider class.
        /// </summary>
        public WeatherDataProvider(string apiKey)
        {
            HttpClient = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(5)
            };
            ApiKey = apiKey;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get current weather forecast data.
        /// </summary>
        public HourlyForecastTransferObject GetCurrentForecast(string cityData)
        {
            return Task.Run(async () => (HourlyForecastTransferObject)await GetForecastDataAsync(WeatherDataType.CurrentWeather, cityData)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get four days weather forecast data.
        /// </summary>
        public FourDaysForecastTransferObject GetFourDaysForecast(string cityData)
        {
            return Task.Run(async () => (FourDaysForecastTransferObject)await GetForecastDataAsync(WeatherDataType.WeatherForecast, cityData)).GetAwaiter().GetResult();
        }

        #endregion

        #region Private methods

        private async Task<IWeatherData> GetForecastDataAsync(WeatherDataType weatherDataType, string input)
        {
            var querry = weatherDataType == WeatherDataType.CurrentWeather ? "weather" : "forecast";
            var call = !int.TryParse(input, out int _) ? "q" : "id";
            var url = new Uri($"http://api.openweathermap.org/data/2.5/{querry}?{call}={input}&units=Metric&appid={ApiKey}&lang=pl").ToString();
            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri(url)
            };

            try
            {
                using (httpRequest)
                {
                    using (HttpResponseMessage httpResponse = await HttpClient.SendAsync(httpRequest))
                    {
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            throw new HttpException($"Operation returned an invalid status code '{httpResponse.StatusCode}'.");
                        }
                        else
                        {
                            string responseContent = await httpResponse.Content.ReadAsStringAsync();

                            return weatherDataType == WeatherDataType.CurrentWeather ? JsonConvert.DeserializeObject<HourlyForecastTransferObject>(responseContent, new CurrentForecastDataConverter())
                                : JsonConvert.DeserializeObject<FourDaysForecastTransferObject>(responseContent, new FourDaysForecastDataConverter()) as IWeatherData;
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw new TaskCanceledException("Unable to get a HTTP response message.", ex);
            }
        }

        #endregion
    }
}
