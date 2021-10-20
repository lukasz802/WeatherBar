using System;
using WebApi.Model.Interfaces;

namespace WebApi.Model.DataTransferObjects
{
    public class DailyForecastTransferObject: IWeatherData
    {
        #region Propeties

        public double MaxTemp { get; set; }

        public double MinTemp { get; set; }

        public string Date { get; set; }

        public string Icon { get; set; }

        public DayOfWeek WeekDay { get; set; }

        public string Description { get; set; }

        public string DescriptionId { get; set; }

        #endregion
    }
}
