using System;
using System.Globalization;
using System.Windows.Data;

namespace GroupCalendar.ViewModel.Converter
{
    internal class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTimeOffset)value;
            return Capitalize(date.ToString("dddd, d/M/yyyy"));
        }

        private object Capitalize(string str)
        {
            return str[..1].ToUpper() + str[1..];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime)value;
            var offset = TimeZoneInfo.Local.BaseUtcOffset;
            return new DateTimeOffset(dateTime, offset);
        }
    }
}
