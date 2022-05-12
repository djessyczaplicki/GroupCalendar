using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GroupCalendar.ViewModel.Converter
{
    internal class RoleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == "Admin" ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
