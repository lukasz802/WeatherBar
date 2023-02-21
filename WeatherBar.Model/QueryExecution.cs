using System.Collections.Generic;

namespace WeatherBar.Model
{
    public class QueryExecution
    {
        #region Properties

        public string Argument { get; set; }

        public IEnumerable<City> Result { get; set; }

        #endregion
    }
}
