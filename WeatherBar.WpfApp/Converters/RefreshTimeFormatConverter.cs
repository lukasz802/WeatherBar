using System;
using System.Globalization;
using System.Windows.Data;
using WeatherBar.Model.Enums;

namespace WeatherBar.WpfApp.Converters
{
    public class RefreshTimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value / 15) - 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (RefreshTime)Enum.ToObject(typeof(RefreshTime), ((int)value + 1) * 15);
        }
    }
}
