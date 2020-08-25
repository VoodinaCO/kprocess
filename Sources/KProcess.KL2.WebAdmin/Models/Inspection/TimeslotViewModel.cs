using System;

namespace KProcess.KL2.WebAdmin.Models.Inspection
{
    public class TimeslotViewModel
    {
        public int TimeslotId { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public TimeSpan StartTime { get; set; }
        public string StartTimeString { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAllDay { get; set; }
        public string EndTimeString { get; set; }
        public string Color { get; set; }
        public int? DisplayOrder { get; set; }
    }
}