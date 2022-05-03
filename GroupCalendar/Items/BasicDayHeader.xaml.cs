using System;
using System.Windows;
using System.Windows.Controls;

namespace GroupCalendar.Items
{
    /// <summary>
    /// Lógica de interacción para BasicDayHeader.xaml
    /// </summary>
    public partial class BasicDayHeader : UserControl
    {
        public DateTimeOffset FirstDay
        {
            get { return (DateTimeOffset)GetValue(FirstDayProperty); }
            set { SetValue(FirstDayProperty, value); }
        }
        public static readonly DependencyProperty FirstDayProperty =
           DependencyProperty.Register("FirstDay", typeof(DateTimeOffset), typeof(BasicDayHeader), new PropertyMetadata(DateTimeOffset.Now));

        public int daysToShow = 7;

        public BasicDayHeader()
        {
            InitializeComponent();
        }

        private void BasicDayHeaderLoaded(object sender, RoutedEventArgs e)
        {
            var today = DateTimeOffset.Now;
            StaticResources.basicDayHeader = this;
            for (int i = 0; i < daysToShow; i++)
            {
                var col = new ColumnDefinition();
                col.Width = new GridLength(1, GridUnitType.Star);
                Grid.ColumnDefinitions.Add(col);
                var text = new TextBlock();
                text.FontWeight = FontWeights.SemiBold;
                text.TextWrapping = TextWrapping.Wrap;
                text.TextAlignment = TextAlignment.Center;
                var day = FirstDay.AddDays(i);
                text.Text = day.ToString("dddd, dd MMMM yyyy");
                Grid.Children.Add(text);
                Grid.SetColumn(text, i);
                if ((day - today).Days == 0)
                {
                    text.TextDecorations = TextDecorations.Underline;
                }
            }

        }

        public void RefreshUi()
        {
            int i = 0;
            var today = DateTimeOffset.Now;
            foreach (object o in Grid.Children)
            {
                TextBlock text = (TextBlock)o;
                if (text == null) return;
                var day = FirstDay.AddDays(i);
                text.Text = day.ToString("dddd, dd MMMM yyyy");
                i++;
                if (day.Date == today.Date)
                {
                    text.TextDecorations = TextDecorations.Underline;
                }
                else
                {
                    text.TextDecorations = null;
                }
            }
        }


    }
}