using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace WeatherBar.Utils
{
    public static class SharedFunctions
    {
        #region Public methods

        public static BitmapImage LoadImage(Stream imageStream)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = imageStream;
            image.EndInit();
            image.Freeze();

            return image;
        }

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
