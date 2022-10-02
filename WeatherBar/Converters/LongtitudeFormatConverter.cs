using System;
using System.Globalization;
using System.Windows.Data;
using WeatherBar.Utils;

namespace WeatherBar.Converters
{
    public class LongtitudeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ViewModelUtils.ConvertCoordinatesFromDecToDeg(System.Convert.ToDouble(value?.ToString().Replace(",", "."), CultureInfo.InvariantCulture), true);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
