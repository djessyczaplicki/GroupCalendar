using GroupCalendar.Data.Remote.Model;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GroupCalendar.ViewModel.Converter
{
    internal class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorModel = value as CustomColorModel;
            if (colorModel != null)
            {
                return Color.FromRgb(System.Convert.ToByte(colorModel.Red),
                                     System.Convert.ToByte(colorModel.Green),
                                     System.Convert.ToByte(colorModel.Blue));
            }
            return Color.FromRgb(255, 100, 100);
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
