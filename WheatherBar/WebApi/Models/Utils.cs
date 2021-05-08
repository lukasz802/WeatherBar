using System;
using System.Linq;

namespace WeatherBar.WebApi.Models
{
    internal static class Utils
    {
        #region Public methods

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(unixTimeStamp);
        }

        public static string ConvertCoordinatesFromDecToDeg(double decValue, bool isLongitude)
        {
            string direction;

            if (decValue > 0)
            {
                if (isLongitude)
                {
                    direction = "E";
                }
                else
                {
                    direction = "N";
                }
            }
            else
            {
                if (isLongitude)
                {
                    direction = "W";
                }
                else
                {
                    direction = "S";
                }
            }

            var temp = Math.Round(decValue > 0 ? decValue : -decValue, 2).ToString().Split('.', ',');
            var minutesValue = Math.Round(double.Parse(temp.Last()) * 60 / 100).ToString();

            return string.Concat(temp.First(), "° ", minutesValue.Length != 1 ? minutesValue : $"0{minutesValue}", $"' {direction}");
        }

        #endregion
    }
}
