using GroupCalendar.Data.Remote.Model;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GroupCalendar.ViewModel.Converter
{
    internal class ColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFromString(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value as Color?;
            if (color != null)
            {
                return new CustomColorModel((Color)color);
            }
            return new CustomColorModel(255, 0, 0);
        }
    }
}
