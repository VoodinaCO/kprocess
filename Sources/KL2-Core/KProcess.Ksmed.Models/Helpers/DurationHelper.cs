using System;

namespace KProcess.Ksmed.Models.Helpers
{
    public static class DurationHelper
    {
        public static string GetTimeScaleMaskValue(long projectTimeScale, long ticks)
        {
            var timeSpan = TimeSpan.FromTicks(ticks);
            var timeSpanHours = Math.Floor(timeSpan.TotalHours);
            var timeSpanMinutes = timeSpan.Minutes;
            var timeSpanSeconds = timeSpan.Seconds;
            var timeSpanMilliseconds = timeSpan.Milliseconds;

            if (projectTimeScale == TimeSpan.FromSeconds(1).Ticks)
                return $"{timeSpanHours.ToString().PadLeft(2, '0')}{timeSpanMinutes.ToString().PadLeft(2, '0')}{timeSpanSeconds.ToString().PadLeft(2, '0')}";
            else if (projectTimeScale == TimeSpan.FromSeconds(.1).Ticks)
                return $"{timeSpanHours.ToString().PadLeft(2, '0')}{timeSpanMinutes.ToString().PadLeft(2, '0')}{timeSpanSeconds.ToString().PadLeft(2, '0')}{timeSpanMilliseconds.ToString().PadLeft(3, '0').Substring(0, 1)}";
            else if (projectTimeScale == TimeSpan.FromSeconds(.01).Ticks)
                return $"{timeSpanHours.ToString().PadLeft(2, '0')}{timeSpanMinutes.ToString().PadLeft(2, '0')}{timeSpanSeconds.ToString().PadLeft(2, '0')}{timeSpanMilliseconds.ToString().PadLeft(3, '0').Substring(0, 2)}";
            else if (projectTimeScale == TimeSpan.FromSeconds(.001).Ticks)
                return $"{timeSpanHours.ToString().PadLeft(2, '0')}{timeSpanMinutes.ToString().PadLeft(2, '0')}{timeSpanSeconds.ToString().PadLeft(2, '0')}{timeSpanMilliseconds.ToString().PadLeft(3, '0')}";
            return null;
        }

        public static long GetTicksValue(long projectTimeScale, string timeScaleMaskValue)
        {
            int timeSpanHours = 0;
            int timeSpanMinutes = 0;
            int timeSpanSeconds = 0;
            int timeSpanMilliseconds = 0;

            if (projectTimeScale == TimeSpan.FromSeconds(1).Ticks)
            {
                var stringValue = $"{timeScaleMaskValue}".PadLeft(6, '0');
                timeSpanHours = int.Parse(stringValue.Substring(0, 2));
                timeSpanMinutes = int.Parse(stringValue.Substring(2, 2));
                timeSpanSeconds = int.Parse(stringValue.Substring(4, 2));
                timeSpanMilliseconds = 0;
            }
            else if (projectTimeScale == TimeSpan.FromSeconds(.1).Ticks)
            {
                var stringValue = $"{timeScaleMaskValue}".PadLeft(7, '0');
                timeSpanHours = int.Parse(stringValue.Substring(0, 2));
                timeSpanMinutes = int.Parse(stringValue.Substring(2, 2));
                timeSpanSeconds = int.Parse(stringValue.Substring(4, 2));
                timeSpanMilliseconds = int.Parse(stringValue.Substring(6, 1));
            }
            else if (projectTimeScale == TimeSpan.FromSeconds(.01).Ticks)
            {
                var stringValue = $"{timeScaleMaskValue}".PadLeft(8, '0');
                timeSpanHours = int.Parse(stringValue.Substring(0, 2));
                timeSpanMinutes = int.Parse(stringValue.Substring(2, 2));
                timeSpanSeconds = int.Parse(stringValue.Substring(4, 2));
                timeSpanMilliseconds = int.Parse(stringValue.Substring(6, 2));
            }
            else if (projectTimeScale == TimeSpan.FromSeconds(.001).Ticks)
            {
                var stringValue = $"{timeScaleMaskValue}".PadLeft(9, '0');
                timeSpanHours = int.Parse(stringValue.Substring(0, 2));
                timeSpanMinutes = int.Parse(stringValue.Substring(2, 2));
                timeSpanSeconds = int.Parse(stringValue.Substring(4, 2));
                timeSpanMilliseconds = int.Parse(stringValue.Substring(6, 3));
            }

            return new TimeSpan(0, timeSpanHours, timeSpanMinutes, timeSpanSeconds, timeSpanMilliseconds).Ticks;
        }
    }
}
