using System.Windows;
using System.Windows.Controls;

namespace GroupCalendar.Items
{
    /// <summary>
    /// Lógica de interacción para BasicSidebarLabel.xaml
    /// </summary>
    public partial class BasicSidebarLabel : UserControl
    {
        int hourHeight = 60;
        public BasicSidebarLabel()
        {
            InitializeComponent();
        }

        private void BasicSidebarLoaded(object sender, RoutedEventArgs e)
        {
            Height = hourHeight * 24;
            for (int hour = 0; hour < 24; hour++)
            {
                var drawnLabel = DrawHourLabel(hour);
                Canvas.Children.Add(drawnLabel);
            }
        }

        private Label DrawHourLabel(int hour)
        {
            Label hourTag = new Label();
            hourTag.Content = FormatHour(hour);
            hourTag.HorizontalAlignment = HorizontalAlignment.Right;
            Canvas.SetTop(hourTag, hour * hourHeight);
            return hourTag;
        }

        private string FormatHour(int hour)
        {
            return hour + ":00";
        }
    }
}
