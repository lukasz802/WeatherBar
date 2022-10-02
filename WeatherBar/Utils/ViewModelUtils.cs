using AppResources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Xml;
using WeatherBar.Core;
using WeatherBar.Model.Enums;
using WeatherBar.Model.Interfaces;

namespace WeatherBar.Utils
{
    public static class ViewModelUtils
    {
        #region Public methods

        public static List<IHourlyData> GetHourlyForecastForSpecificDate(List<IHourlyData> hourlyData, Language language, DateTime date)
        {
            var cultureName = new CultureInfo(language == Language.English ? "en-US" : "pl-PL");
            var tempDate = date.ToString("dd MMMM", cultureName).Trim();

            return hourlyData.Where(x => x.Date.Contains(tempDate.First() == '0' ? tempDate.Remove(0, 1) : tempDate)).ToList();
        }

        public static string ConvertCoordinatesFromDecToDeg(double decValue, bool isLongitude)
        {
            string direction = decValue > 0 ? isLongitude ? "E" : "N" : isLongitude ? "W" : "S";
            string[] temp = Math.Round(decValue > 0 ? decValue : -decValue, 2).ToString().Split('.', ',');
            string minutesValue = Math.Round(double.Parse(temp.Last()) * 60 / 100).ToString();

            return string.Concat(temp.First(), "° ", minutesValue.Length != 1 ? minutesValue : $"0{minutesValue}", $"' {direction}");
        }

        public static Tuple<List<IHourlyData>, List<IHourlyData>> GetHourlyForecastFromHourlyData(List<IHourlyData> hourlyData)
        {
            return new Tuple<List<IHourlyData>, List<IHourlyData>>(hourlyData.Take(5).ToList(), hourlyData.ToList().GetRange(5, 5).ToList());
        }

        public static BitmapImage LoadImage(Stream imageStream)
        {
            var image = new BitmapImage();

            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = imageStream;
            image.EndInit();
            image.Freeze();

            return image;
        }

        public static AppResource GetXmlResource<T>(T resourceType) where T : struct, IConvertible
        {
            if (!(resourceType is Language || resourceType is Units))
            {
                throw new ArgumentException("T must be an Language or Units enumerated type");
            }

            var document = new XmlDocument();
            document.Load(resourceType is Language ? ResourceManager.GetLanguage(resourceType.ToString()) : ResourceManager.GetUnits(resourceType.ToString()));

            return new AppResource(document);
        }

        #endregion
    }
}
