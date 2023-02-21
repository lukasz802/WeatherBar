using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WeatherBar.WpfApp.Converters
{
    public class LongtitudeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double decValue = System.Convert.ToDouble(value?.ToString().Replace(".", ",") ?? default);
            string direction = decValue > 0 ? "E" : "W";
            string[] temp = Math.Round(decValue > 0 ? decValue : -decValue, 2).ToString().Split('.', ',');
            string minutesValue = Math.Round(double.Parse(temp.Last()) * 60 / 100).ToString();

            return string.Concat(temp.First(), "° ", minutesValue.Length != 1 ? minutesValue : $"0{minutesValue}", $"' {direction}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
