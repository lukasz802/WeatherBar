using WebApi.Models.Interfaces;

namespace WebApi.Models.DataTransferObjects
{
    public class DailyForecastTransferObject : IDailyData
    {
        #region Propeties

        public int MaxTemp { get; set; }

        public int MinTemp { get; set; }

        public string Date { get; set; }

        public string Icon { get; set; }

        public string Description { get; set; }

        #endregion

        #region Construcors

        public DailyForecastTransferObject()
        {
        }

        #endregion
    }
}
