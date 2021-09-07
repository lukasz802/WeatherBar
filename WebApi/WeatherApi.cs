using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApi.Models.Enums;
using WebApi.Models.Interfaces;
using WebApi.Models.Converters;
using System.Configuration;

namespace WebApi
{
    public class WeatherApi : ServiceClient<WeatherApi>, IWeatherApi
    {
        #region Fields and constants

        private int interval = 15;

        #endregion

        #region Properties

        public string ApiKey { get; private set; }

        public Units Units { get; set; }

        public string CityName { get; set; }

        public int Interval 
        { 
            get
            {
                return interval;
            }
            set
            {
                if (value < 15)
                {
                    interval = 15;
                }
                else if (value > 60)
                {
                    interval = 60;
                }
                else
                {
                    interval = value;
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the WeatherAPI class.
        /// </summary>
        public WeatherApi() : base()
        {
            HttpClient.Timeout = TimeSpan.FromSeconds(5);
            SetApiConfiguration();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get current weather data by city name.
        /// </summary>
        public IHourlyData GetCurrentWeatherDataByCityName(string cityName)
        {
            return Task.Run(() => GetCurrentWeatherDataBodyAsync(CallType.ByCityName, cityName)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get current weather data by city ID.
        /// </summary>
        public IHourlyData GetCurrentWeatherDataByCityId(string cityId)
        {
            return Task.Run(() => GetCurrentWeatherDataBodyAsync(CallType.ByCityID, cityId)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get four days forecast data by city name.
        /// </summary>
        public IFourDaysData GetFourDaysForecastDataByCityName(string cityName)
        {
            return Task.Run(() => GetWeatherForecastDataBodyAsync(CallType.ByCityName, cityName)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get four days forecast data by city ID.
        /// </summary>
        public IFourDaysData GetFourDaysForecastDataByCityId(string cityId)
        {
            return Task.Run(() => GetWeatherForecastDataBodyAsync(CallType.ByCityID, cityId)).GetAwaiter().GetResult();
        }

        #endregion

        #region Methods

        private async Task<IWeatherData> GetForecastDataAsync(WeatherDataType weatherDataType, CallType callType, string input)
        {
            var querry = weatherDataType == WeatherDataType.CurrentWeather ? "weather" : "forecast";
            var call = callType == CallType.ByCityName ? "q" : "id";
            var url = new Uri($"http://api.openweathermap.org/data/2.5/{querry}?{call}={input}&units={Units}&appid={ApiKey}&lang=pl").ToString();
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

        private async Task<IHourlyData> GetCurrentWeatherDataBodyAsync(CallType callType, string input)
        {
            return (IHourlyData) await GetForecastDataAsync(WeatherDataType.CurrentWeather, callType,  input);
        }

        private async Task<IFourDaysData> GetWeatherForecastDataBodyAsync(CallType callType, string input)
        {
            return (IFourDaysData) await GetForecastDataAsync(WeatherDataType.WeatherForecast, callType, input);
        }

        private void SetApiConfiguration()
        {
            string[] apiKeysArray = ConfigurationManager.AppSettings.Get("ApiKeys").Replace(" ", string.Empty).Split(',');

            ApiKey = apiKeysArray[new Random().Next(0, apiKeysArray.Length)];
            Units = (Units)Enum.Parse(typeof(Units), ConfigurationManager.AppSettings.Get("Units"));
            Interval = int.Parse(ConfigurationManager.AppSettings.Get("Interval"));
            CityName = ConfigurationManager.AppSettings.Get("CityName");
        }

        #endregion
    }
}
