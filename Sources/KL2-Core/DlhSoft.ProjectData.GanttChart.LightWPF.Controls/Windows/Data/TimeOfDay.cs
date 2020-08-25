namespace DlhSoft.Windows.Data
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    // Ajout Tekigo : type covnerter
    [StructLayout(LayoutKind.Sequential)]
    [TypeConverter(typeof(TimeOfDayConverter))]
    public struct TimeOfDay : IEquatable<TimeOfDay>, IComparable<TimeOfDay>, IComparable
    {
        private TimeSpan timeOfDay;
        private static readonly TimeSpan endOfDay;
        public static readonly TimeOfDay MaxValue;
        public static readonly TimeOfDay MinValue;
        public TimeOfDay(TimeSpan timeOfDay)
        {
            if (timeOfDay <= TimeSpan.Zero)
            {
                timeOfDay = TimeSpan.Zero;
            }
            if (timeOfDay >= endOfDay)
            {
                timeOfDay = endOfDay;
            }
            this.timeOfDay = timeOfDay;
        }

        public TimeSpan ToTimeSpan()
        {
            return this.timeOfDay;
        }

        public static implicit operator TimeSpan(TimeOfDay timeOfDay)
        {
            return timeOfDay.ToTimeSpan();
        }

        public static implicit operator TimeOfDay(TimeSpan timeOfDay)
        {
            return new TimeOfDay(timeOfDay);
        }

        public TimeOfDay(long ticks)
            : this(new TimeSpan(ticks))
        {
        }

        public TimeOfDay(int hours, int minutes, int seconds)
            : this(new TimeSpan(hours, minutes, seconds))
        {
        }

        public TimeOfDay(int days, int hours, int minutes, int seconds)
            : this(new TimeSpan(days, hours, minutes, seconds))
        {
        }

        public TimeOfDay(int days, int hours, int minutes, int seconds, int milliseconds)
            : this(new TimeSpan(days, hours, minutes, seconds, milliseconds))
        {
        }

        public TimeSpan Add(TimeSpan value)
        {
            return this.timeOfDay.Add(value);
        }

        public static int Compare(TimeOfDay d1, TimeOfDay d2)
        {
            return TimeSpan.Compare(d1.timeOfDay, d2.timeOfDay);
        }

        public int CompareTo(TimeOfDay value)
        {
            return this.timeOfDay.CompareTo(value.timeOfDay);
        }

        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }
            if (value is TimeOfDay)
            {
                return this.CompareTo((TimeOfDay)value);
            }
            return this.timeOfDay.CompareTo(value);
        }

        public bool Equals(TimeOfDay value)
        {
            return this.timeOfDay.Equals(value.timeOfDay);
        }

        public override bool Equals(object value)
        {
            if (value == null)
            {
                return false;
            }
            if (value is TimeOfDay)
            {
                return this.Equals((TimeOfDay)value);
            }
            return this.timeOfDay.Equals(value);
        }

        public static bool Equals(TimeOfDay d1, TimeOfDay d2)
        {
            return TimeSpan.Equals((TimeSpan)d1, (TimeSpan)d2);
        }

        public static TimeOfDay FromDays(double value)
        {
            return new TimeOfDay(TimeSpan.FromDays(value));
        }

        public static TimeOfDay FromHours(double value)
        {
            return new TimeOfDay(TimeSpan.FromHours(value));
        }

        public static TimeOfDay FromMilliseconds(double value)
        {
            return new TimeOfDay(TimeSpan.FromMilliseconds(value));
        }

        public static TimeOfDay FromMinutes(double value)
        {
            return new TimeOfDay(TimeSpan.FromMinutes(value));
        }

        public static TimeOfDay FromSeconds(double value)
        {
            return new TimeOfDay(TimeSpan.FromSeconds(value));
        }

        public static TimeOfDay FromTicks(long value)
        {
            return new TimeOfDay(TimeSpan.FromTicks(value));
        }

        public override int GetHashCode()
        {
            return this.timeOfDay.GetHashCode();
        }

        public TimeSpan Negate()
        {
            return this.timeOfDay.Negate();
        }

        public static TimeOfDay Parse(string s)
        {
            return new TimeOfDay(TimeSpan.Parse(s));
        }

        public TimeSpan Subtract(TimeSpan value)
        {
            return this.timeOfDay.Subtract(value);
        }

        public override string ToString()
        {
            return this.timeOfDay.ToString();
        }

        public static bool TryParse(string s, out TimeOfDay result)
        {
            TimeSpan span;
            bool flag = TimeSpan.TryParse(s, out span);
            result = new TimeOfDay(span);
            return flag;
        }

        public static TimeSpan operator +(TimeOfDay d)
        {
            return +d.timeOfDay;
        }

        public static TimeSpan operator +(TimeOfDay d, TimeSpan t)
        {
            return (d.timeOfDay + t);
        }

        public static bool operator ==(TimeOfDay d1, TimeSpan d2)
        {
            return (d1.timeOfDay == d2);
        }

        public static bool operator !=(TimeOfDay d1, TimeSpan d2)
        {
            return (d1.timeOfDay != d2);
        }

        public static bool operator >(TimeOfDay d1, TimeSpan d2)
        {
            return (d1.timeOfDay > d2);
        }

        public static bool operator >=(TimeOfDay d1, TimeSpan d2)
        {
            return (d1.timeOfDay >= d2);
        }

        public static bool operator <(TimeOfDay d1, TimeSpan d2)
        {
            return (d1.timeOfDay < d2);
        }

        public static bool operator <=(TimeOfDay d1, TimeSpan d2)
        {
            return (d1.timeOfDay <= d2);
        }

        public static TimeSpan operator -(TimeOfDay d)
        {
            return -d.timeOfDay;
        }

        public static TimeSpan operator -(TimeOfDay d, TimeSpan t)
        {
            return (d.timeOfDay - t);
        }

        public int Days
        {
            get
            {
                return this.timeOfDay.Days;
            }
        }
        public int Hours
        {
            get
            {
                return this.timeOfDay.Hours;
            }
        }
        public int Milliseconds
        {
            get
            {
                return this.timeOfDay.Milliseconds;
            }
        }
        public int Minutes
        {
            get
            {
                return this.timeOfDay.Minutes;
            }
        }
        public int Seconds
        {
            get
            {
                return this.timeOfDay.Seconds;
            }
        }
        public long Ticks
        {
            get
            {
                return this.timeOfDay.Ticks;
            }
        }
        public double TotalDays
        {
            get
            {
                return this.timeOfDay.TotalDays;
            }
        }
        public double TotalHours
        {
            get
            {
                return this.timeOfDay.TotalHours;
            }
        }
        public double TotalMilliseconds
        {
            get
            {
                return this.timeOfDay.TotalMilliseconds;
            }
        }
        public double TotalMinutes
        {
            get
            {
                return this.timeOfDay.TotalMinutes;
            }
        }
        public double TotalSeconds
        {
            get
            {
                return this.timeOfDay.TotalSeconds;
            }
        }
        static TimeOfDay()
        {
            endOfDay = TimeSpan.Parse("1.00:00:00");
            MaxValue = new TimeOfDay(endOfDay);
            MinValue = new TimeOfDay(TimeSpan.Zero);
        }
    }
}

