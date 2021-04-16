namespace WeatherBar.Models
{
    public class City
    {
        #region Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public decimal Longtitude { get; set; }

        public decimal Latitude { get; set; }

        #endregion
    }
}
