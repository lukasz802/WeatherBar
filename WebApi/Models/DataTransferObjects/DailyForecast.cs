using WeatherBar.WebApi.Models.Interfaces;

namespace WeatherBar.WebApi.Models.DataTransferObjects
{
    public class DailyForecast : IDailyData
    {
        #region Propeties

        public int MaxTemp { get; set; }

        public int MinTemp { get; set; }

        public string Date { get; set; }

        public string Icon { get; set; }

        public string Description { get; set; }

        #endregion

        #region Construcors

        public DailyForecast()
        {
        }

        #endregion
    }
}
