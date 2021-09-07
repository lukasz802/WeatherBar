using System;
using System.Globalization;
using System.Windows.Data;

namespace WeatherBar.Converters
{
    public class LongtitudeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return WebApi.Models.Utils.ConvertCoordinatesFromDecToDeg(System.Convert.ToDouble(value), true);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
