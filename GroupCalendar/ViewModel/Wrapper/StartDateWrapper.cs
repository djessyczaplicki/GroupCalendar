using System;
using System.Windows;

namespace GroupCalendar.ViewModel.Wrapper
{
    public class StartDateWrapper : DependencyObject
    {
        private static readonly DependencyProperty StartDateProperty =
         DependencyProperty.Register("StartDate", typeof(DateTime),
         typeof(StartDateWrapper), new FrameworkPropertyMetadata(DateTime.Now));

        public DateTime StartDate
        {
            get { return (DateTime)GetValue(StartDateProperty); }
            set { SetValue(StartDateProperty, value); }
        }
    }
}
