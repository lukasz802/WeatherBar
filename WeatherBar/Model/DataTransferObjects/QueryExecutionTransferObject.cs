using System.Collections.Generic;

namespace WeatherBar.Model.DataTransferObjects
{
    public class QueryExecutionTransferObject
    {
        #region Properties

        public string Argument { get; set; }

        public IEnumerable<City> Result { get; set; }

        #endregion
    }
}
