using System;

namespace GroupCalendar.Data.Local
{
    internal class WeekDayCheck
    {
        public DayOfWeek DayOfWeek { get; set; }
        public string Day { get; set; } = "";
        public bool IsChecked { get; set; }
    }
}
