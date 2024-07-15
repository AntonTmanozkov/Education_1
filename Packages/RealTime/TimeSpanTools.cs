using System;

namespace RealTimeSystem
{
    public static class TimeSpanTools
    {
        public static int GetSeconds(TimeSpan span)
        {
            return (int)Math.Ceiling(span.TotalSeconds - span.Hours * 60 * 60 - span.Minutes * 60);
        }

        public static string ConvertToTime(this TimeSpan span)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)span.TotalHours, span.Minutes, GetSeconds(span));
        }
    }
}