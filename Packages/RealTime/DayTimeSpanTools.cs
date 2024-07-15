using System;
using UnityEngine;

namespace RealTimeSystem
{
    public static class DayTimeSpanTools
    {
        internal static DateTime GetDefault(DayTimeSpan span)
        {
            int today = DateTime.UtcNow.Day;
            int yesterday = today - 1;
            int day = DateTime.UtcNow.Hour < span.Hours ? yesterday : today;

            int year = DateTime.UtcNow.Year;
            int month = DateTime.UtcNow.Month;

            // if today was the first day of the month
            if (day == 0)
            {
                month -= 1;
                int maxDaysInMonth = DateTime.DaysInMonth(year, month);
                day = Mathf.Clamp(31, 1, maxDaysInMonth);
            }

            return new DateTime(year,
                month,
                day,
                span.Hours,
                span.Minutes,
                span.Seconds,
                DateTimeKind.Utc);
        }
    }
}