using System;
using System.Globalization;
using System.Windows.Data;

namespace GroupCalendar.ViewModel.Converter
{
    internal class BoolToSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? 1 : 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
