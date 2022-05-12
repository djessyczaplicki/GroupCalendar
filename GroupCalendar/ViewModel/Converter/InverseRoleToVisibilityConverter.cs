using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GroupCalendar.ViewModel.Converter
{
    internal class InverseRoleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == "Admin" ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
