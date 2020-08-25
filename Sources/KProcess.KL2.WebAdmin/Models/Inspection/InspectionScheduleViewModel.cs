using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Inspection
{
    public class InspectionScheduleViewModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }


        public int ProcessId { get; set; }
        public string ProcessLabel { get; set; }
        //Mandatory in Syncfusion Schedule
        public DateTime StartTime { get; set; }
        //Mandatory in Syncfusion Schedule
        public DateTime EndTime { get; set; }
        //Used for Recurrence in Syncfusion Schedule
        public int? RecurrenceID { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
        //Used for IsAllDay event in Syncfusion Schedule
        public bool IsAllDay { get; set; }

        public int TimeslotId { get; set; }
        public TimeslotViewModel Timeslot { get; set; }
        
    }
}