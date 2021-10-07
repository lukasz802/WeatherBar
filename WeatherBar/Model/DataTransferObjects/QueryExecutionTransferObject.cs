using System.Collections.ObjectModel;

namespace WeatherBar.Model.DataTransferObjects
{
    public class QueryExecutionTransferObject
    {
        #region Properties

        public string Argument { get; set; }

        public ObservableCollection<City> Result { get; set; }

        #endregion
    }
}
