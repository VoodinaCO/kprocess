namespace DlhSoft.Windows.Data
{
    using System;
    using System.Runtime.CompilerServices;

    public class TimeInterval
    {
        public TimeInterval(DateTime start, DateTime finish)
        {
            if (finish < start)
            {
                throw new InvalidOperationException("Specified finish date and time must be greater than the specified start date and time.");
            }
            this.Start = start;
            this.Finish = finish;
        }

        public TimeSpan Duration
        {
            get
            {
                return (TimeSpan) (this.Finish - this.Start);
            }
        }

        public DateTime Finish { get; private set; }

        public DateTime Start { get; private set; }
    }
}

