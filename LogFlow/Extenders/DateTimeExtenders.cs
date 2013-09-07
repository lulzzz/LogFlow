using System;
using System.Globalization;

namespace LogFlow
{
    public static class DateTimeExtenders
    {
        public static int WeekOfYear(this DateTime value)
        {
            var cc = CultureInfo.CurrentCulture;

            return cc.Calendar.GetWeekOfYear(
                value, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
