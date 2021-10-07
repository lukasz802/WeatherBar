using System;
using System.Globalization;
using System.Windows.Data;

namespace WeatherBar.Converters
{
    public class LatitudeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return WebApi.Model.Utils.ConvertCoordinatesFromDecToDeg(System.Convert.ToDouble(value), false);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
