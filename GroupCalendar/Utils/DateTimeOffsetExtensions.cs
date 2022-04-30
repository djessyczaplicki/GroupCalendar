using System;

namespace GroupCalendar.Utils
{
    public static class DateTimeOffsetExtensions
    {
        public static DateTimeOffset StartOfWeek(this DateTimeOffset dateTimeOffset, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dateTimeOffset.DayOfWeek - startOfWeek)) % 7;
            return dateTimeOffset.AddDays(-1 * diff).Date;
        }
    }
}
