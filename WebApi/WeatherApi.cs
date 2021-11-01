using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApi.Model.Enums;
using WebApi.Model.Interfaces;
using WebApi.Model.Converters;
using WebApi.Model.DataTransferObjects;

namespace WebApi
{
    public class WeatherApi : ServiceClient<WeatherApi>, IWeatherApi
    {
        #region Properties

        public string ApiKey { get; }

        public string CityId { get; private set; }

        public int Interval { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the WeatherAPI class.
        /// </summary>
        public WeatherApi(string apiKey, string cityId, int interval) : base()
        {
            HttpClient.Timeout = TimeSpan.FromSeconds(5);
            ApiKey = apiKey;
            CityId = cityId;
            Interval = interval;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get current weather data by city name.
        /// </summary>
        public HourlyForecastTransferObject GetCurrentWeatherDataByCityName(string cityName)
        {
            return Task.Run(() => GetCurrentWeatherDataBodyAsync(CallType.ByCityName, cityName)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get current weather data by city ID.
        /// </summary>
        public HourlyForecastTransferObject GetCurrentWeatherDataByCityId(string cityId)
        {
            return Task.Run(() => GetCurrentWeatherDataBodyAsync(CallType.ByCityID, cityId)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get four days forecast data by city name.
        /// </summary>
        public FourDaysForecastTransferObject GetFourDaysForecastDataByCityName(string cityName)
        {
            return Task.Run(() => GetWeatherForecastDataBodyAsync(CallType.ByCityName, cityName)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get four days forecast data by city ID.
        /// </summary>
        public FourDaysForecastTransferObject GetFourDaysForecastDataByCityId(string cityId)
        {
            return Task.Run(() => GetWeatherForecastDataBodyAsync(CallType.ByCityID, cityId)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Update WeatherAPI configuration.
        /// </summary>
        public void UpdateConfiguration(string cityId, int interval)
        {
            CityId = cityId;
            Interval = interval;
        }

        #endregion

        #region Private methods

        private async Task<IWeatherData> GetForecastDataAsync(WeatherDataType weatherDataType, CallType callType, string input)
        {
            var querry = weatherDataType == WeatherDataType.CurrentWeather ? "weather" : "forecast";
            var call = callType == CallType.ByCityName ? "q" : "id";
            var url = new Uri($"http://api.openweathermap.org/data/2.5/{querry}?{call}={input}&units=Metric&appid={ApiKey}&lang=pl").ToString();
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
                            return weatherDataType == WeatherDataType.CurrentWeather ? SafeJsonConvert.DeserializeObject<HourlyForecastTransferObject>(responseContent, new CurrentWeatherDataConverter())
                                : SafeJsonConvert.DeserializeObject<FourDaysForecastTransferObject>(responseContent, new FourDaysForecastDataConverter()) as IWeatherData;
                        }
                        catch (JsonException ex)
                        {
                            httpRequest.Dispose();
                            throw new SerializationException("Unable to deserialize the response", responseContent, ex);
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw new TaskCanceledException("Unable to get a HTTP response message", ex);
            }
        }

        private async Task<HourlyForecastTransferObject> GetCurrentWeatherDataBodyAsync(CallType callType, string input)
        {
            return (HourlyForecastTransferObject)await GetForecastDataAsync(WeatherDataType.CurrentWeather, callType, input);
        }

        private async Task<FourDaysForecastTransferObject> GetWeatherForecastDataBodyAsync(CallType callType, string input)
        {
            return (FourDaysForecastTransferObject)await GetForecastDataAsync(WeatherDataType.WeatherForecast, callType, input);
        }

        #endregion
    }
}
