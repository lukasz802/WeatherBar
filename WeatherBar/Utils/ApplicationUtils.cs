using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace WeatherBar.Utils
{
    public static class ApplicationUtils
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

        public static T FindVisualParent<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(depObj);

                if (parent == null)
                {
                    return null;
                }
                else if (parent is T)
                {
                    return (T)parent;
                }
                else
                {
                    return FindVisualParent<T>(parent);
                }
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static string GetDescriptionFromId(string descriptionId)
        {
            switch (descriptionId)
            {
                case "200":
                    return "Thunderstorm with light rain";
                case "201":
                    return "Thunderstorm with rain";
                case "202":
                    return "Thunderstorm with heavy rain";
                case "210":
                    return "Light thunderstorm";
                case "211":
                    return "Thunderstorm";
                case "212":
                    return "Heavy thunderstorm";
                case "221":
                    return "Ragged thunderstorm";
                case "230":
                    return "Thunderstorm with light drizzle";
                case "231":
                    return "Thunderstorm with drizzle";
                case "232":
                    return "Thunderstorm with heavy drizzle";
                case "300":
                    return "Light intensity drizzle";
                case "301":
                    return "Drizzle";
                case "302":
                    return "Heavy intensity drizzle";
                case "310":
                    return "Light intensity drizzle rain";
                case "311":
                    return "Drizzle rain";
                case "312":
                    return "Heavy intensity drizzle rain";
                case "313":
                    return "Shower rain and drizzle";
                case "314":
                    return "Heavy shower rain and drizzle";
                case "321":
                    return "Shower drizzle";
                case "500":
                    return "Light rain";
                case "501":
                    return "Moderate rain";
                case "502":
                    return "Heavy intensity rain";
                case "503":
                    return "Very heavy rain";
                case "504":
                    return "Extreme rain";
                case "511":
                    return "Freezing rain";
                case "520":
                    return "Light intensity shower rain";
                case "521":
                    return "Shower rain";
                case "522":
                    return "Heavy intensity shower rain";
                case "531":
                    return "Ragged shower rain";
                case "600":
                    return "Light snow";
                case "601":
                    return "Snow";
                case "602":
                    return "Heavy snow";
                case "611":
                    return "Sleet";
                case "612":
                    return "Light shower sleet";
                case "613":
                    return "Shower sleet";
                case "615":
                    return "Light rain and snow";
                case "616":
                    return "Rain and snow";
                case "620":
                    return "Light shower snow";
                case "621":
                    return "Shower snow";
                case "622":
                    return "Heavy shower snow";
                case "701":
                    return "Mist";
                case "711":
                    return "Smoke";
                case "721":
                    return "Haze";
                case "731":
                    return "Dust whirls";
                case "741":
                    return "Fog";
                case "751":
                    return "Sand";
                case "761":
                    return "Dust";
                case "762":
                    return "Volcanic ash";
                case "771":
                    return "Squalls";
                case "781":
                    return "Tornado";
                case "800":
                    return "Clear sky";
                case "801":
                    return "Few clouds";
                case "802":
                    return "Scattered clouds";
                case "803":
                    return "Broken clouds";
                case "804":
                    return "Overcast clouds";
                default:
                    throw new ArgumentException($"Invalid input data. There is no appropriate content for Id: {descriptionId}");
            }
        }

        #endregion
    }
}
