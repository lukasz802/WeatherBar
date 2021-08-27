using System;
using System.Linq;

namespace WebApi.Models
{
    public static class Utils
    {
        #region Public methods

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(unixTimeStamp);
        }

        public static string ConvertCoordinatesFromDecToDeg(double decValue, bool isLongitude)
        {
            string direction = decValue > 0 ? isLongitude ? "E" : "N" : isLongitude ? "W" : "S";
            string[] temp = Math.Round(decValue > 0 ? decValue : -decValue, 2).ToString().Split('.', ',');
            string minutesValue = Math.Round(double.Parse(temp.Last()) * 60 / 100).ToString();

            return string.Concat(temp.First(), "° ", minutesValue.Length != 1 ? minutesValue : $"0{minutesValue}", $"' {direction}");
        }

        #endregion
    }
}
