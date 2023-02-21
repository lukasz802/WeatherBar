using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using WeatherBar.DataProviders.Converters;
using WeatherBar.DataProviders.Interfaces;
using WeatherBar.Model;

namespace WeatherBar.DataProviders
{
    public class WeatherDataProvider : IWeatherDataProvider
    {
        #region Fields

        private readonly HttpClient httpClient;

        private readonly string apiKey;

        #endregion

        #region Constructor

        public WeatherDataProvider(string apiKey)
        {
            httpClient = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(5)
            };
            this.apiKey = apiKey;
        }

        #endregion

        #region Public methods

        public HourlyForecast GetCurrentForecast(string cityData)
        {
            return GetForecastDataAsync<HourlyForecast>("weather", cityData, new CurrentForecastDataConverter()).GetAwaiter().GetResult();
        }

        public FourDaysForecast GetFourDaysForecast(string cityData)
        {
            return GetForecastDataAsync<FourDaysForecast>("forecast", cityData, new FourDaysForecastDataConverter()).GetAwaiter().GetResult();
        }

        #endregion

        #region Private methods

        private async Task<T> GetForecastDataAsync<T>(string query, string input, JsonConverter converter)
        {
            var call = !int.TryParse(input, out int _) ? "q" : "id";
            var url = new Uri($"http://api.openweathermap.org/data/2.5/{query}?{call}={input}&units=Metric&appid={apiKey}&lang=pl").ToString();
            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri(url)
            };

            try
            {
                using (httpRequest)
                {
                    using (HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest))
                    {
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            throw new HttpException($"Operation returned an invalid status code '{httpResponse.StatusCode}'.");
                        }
                        else
                        {
                            string responseContent = await httpResponse.Content.ReadAsStringAsync();

                            return JsonConvert.DeserializeObject<T>(responseContent, converter);
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
