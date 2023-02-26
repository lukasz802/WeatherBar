using System;

namespace WeatherBar.Utils.Extensions
{
    public static class LongExtensions
    {
        #region Public methods

        public static DateTime ToDateTime(this long unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(unixTimeStamp);
        }

        #endregion
    }
}
