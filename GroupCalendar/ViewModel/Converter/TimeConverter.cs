using System;
using System.Globalization;
using System.Windows.Data;

namespace GroupCalendar.ViewModel.Converter
{
    internal class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTimeOffset)value;
            if (dateTime == null)
            {
                return "";
            }
            return dateTime.Hour + ":" + Pad(dateTime.Minute);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        private string Pad(int num)
        {
            var str = num.ToString();
            return (str.Length == 1) ? "0" + str : str;
        }
    }
}
