using System;
using System.Globalization;
using System.Windows.Data;

namespace WeatherBar.WpfApp.Converters.Template
{
    public abstract class EnumFormatConverterBase<T> : IValueConverter where T : Enum
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (T)Enum.ToObject(typeof(T), (int)value);
        }
    }
}
