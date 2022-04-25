using System;
using System.Globalization;
using System.Windows.Data;

namespace GroupCalendar.ViewModel.Converter
{
    internal class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTimeOffset)value;
            return date;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime)value;
            var offset = TimeZoneInfo.Local.BaseUtcOffset;
            return new DateTimeOffset(dateTime, offset);
        }
    }
}
