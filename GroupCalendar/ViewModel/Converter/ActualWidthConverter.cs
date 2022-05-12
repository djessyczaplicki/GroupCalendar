using System;
using System.Globalization;
using System.Windows.Data;

namespace GroupCalendar.ViewModel.Converter
{
    internal class ActualWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Max(((double)value) - 18, 20);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
