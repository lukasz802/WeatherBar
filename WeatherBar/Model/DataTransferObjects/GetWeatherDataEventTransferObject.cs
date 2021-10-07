using WebApi.Model.Enums;

namespace WeatherBar.Model.DataTransferObjects
{
    public class GetWeatherDataEventTransferObject
    {
        #region Properties

        public string Argument { get; set; }

        public CallType CallType { get; set; }

        public bool IsRefreshIndicatorVisible { get; set; }

        #endregion
    }
}
