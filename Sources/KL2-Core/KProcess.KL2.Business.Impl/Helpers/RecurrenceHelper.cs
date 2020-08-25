using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using KProcess.Ksmed.Models;

namespace KProcess.KL2.Business.Impl.Helpers
{
    public static class RecurrenceHelper
    {
        public static IEnumerable<DateTime> GetRecurrenceDateTimeCollection(InspectionSchedule inspectionSchedule, DateTime? StartDateFilter = null, DateTime? EndDateFilter = null)
        {
            if (inspectionSchedule == null)
                throw new ArgumentNullException(nameof(inspectionSchedule));
            if (inspectionSchedule.Timeslot == null)
                throw new ArgumentNullException($"{nameof(inspectionSchedule)}.{nameof(InspectionSchedule.Timeslot)}");

            var excludeDates = inspectionSchedule.RecurrenceException?.Split(',').Select(_ => DateTime.ParseExact(_.Split('T')[0], "yyyyMMdd", CultureInfo.InvariantCulture)).ToList();
            var inspectionScheduleStartDateTime = new DateTime(inspectionSchedule.StartDate.Year,
                inspectionSchedule.StartDate.Month,
                inspectionSchedule.StartDate.Day,
                inspectionSchedule.Timeslot.StartTime.Hours,
                inspectionSchedule.Timeslot.StartTime.Minutes,
                inspectionSchedule.Timeslot.StartTime.Seconds);

            return GetRecurrenceDateTimeCollection(inspectionSchedule.RecurrenceRule, inspectionScheduleStartDateTime, StartDateFilter, EndDateFilter, excludeDates);
        }

        public static IEnumerable<DateTime> GetRecurrenceDateTimeCollection(string RRule, DateTime RecStartDate, DateTime? StartDateFilter = null, DateTime? EndDateFilter = null, IEnumerable<DateTime> excludeDate = null)
        {
            string COUNT;
            string RECCOUNT;
            string DAILY;
            string WEEKLY;
            string MONTHLY;
            string YEARLY;
            string INTERVAL;
            string INTERVALCOUNT;
            string BYSETPOS;
            string BYSETPOSCOUNT;
            string BYDAY;
            string BYDAYVALUE;
            string BYMONTHDAY;
            string BYMONTHDAYCOUNT;
            string BYMONTH;
            string BYMONTHCOUNT;
            int BYDAYPOSITION;
            int BYMONTHDAYPOSITION = 32;
            int WEEKLYBYDAYPOS = 8;
            string WEEKLYBYDAY;
            string EXDATE;

            List<DateTime> exDateList = excludeDate == null ? new List<DateTime>()
                : excludeDate.Select(_ => _.Date).ToList();

            void FindExdateList(string[] param)
            {
                for (int i = 0; i < param.Length; i++)
                {
                    if (param[i].Contains("EXDATE"))
                    {
                        EXDATE = param[i];
                        var _rule = param[i].Split('=');
                        if (_rule[0] == "EXDATE")
                        {
                            var exDates = _rule[1].Split(',');
                            for (var j = 0; j < exDates.Length; j++)
                                exDateList.Add(DateTime.ParseExact(exDates[j], "dd/MM/yyyy", CultureInfo.InvariantCulture));
                        }
                        break;
                    }
                }
            }

            int GetWeekDay(string param)
            {
                switch (param)
                {
                    case "SU":
                        return 1;
                    case "MO":
                        return 2;
                    case "TU":
                        return 3;
                    case "WE":
                        return 4;
                    case "TH":
                        return 5;
                    case "FR":
                        return 6;
                    case "SA":
                        return 7;
                }
                return 8;
            }

            void FindWeeklyRule(string[] param)
            {
                for (int i = 0; i < param.Length; i++)
                {
                    if (param[i].Contains("BYDAY"))
                    {
                        WEEKLYBYDAY = param[i];
                        WEEKLYBYDAYPOS = i;
                        break;
                    }
                }
            }

            void FindKeyIndex(string[] param)
            {
                RECCOUNT = "";
                DAILY = "";
                WEEKLY = "";
                MONTHLY = "";
                YEARLY = "";
                BYSETPOS = "";
                BYSETPOSCOUNT = "";
                INTERVAL = "";
                INTERVALCOUNT = "";
                COUNT = "";
                BYDAY = "";
                BYDAYVALUE = "";
                BYMONTHDAY = "";
                BYMONTHDAYCOUNT = "";
                BYMONTH = "";
                BYMONTHCOUNT = "";
                WEEKLYBYDAY = "";

                for (int i = 0; i < param.Length; i++)
                {
                    if (param[i].Contains("COUNT"))
                    {
                        COUNT = param[i];
                        RECCOUNT = param[i + 1];
                    }
                    if (param[i].Contains("DAILY"))
                        DAILY = param[i];
                    if (param[i].Contains("WEEKLY"))
                        WEEKLY = param[i];
                    if (param[i].Contains("INTERVAL"))
                    {
                        INTERVAL = param[i];
                        INTERVALCOUNT = param[i + 1];
                    }
                    if (param[i].Contains("MONTHLY"))
                        MONTHLY = param[i];
                    if (param[i].Contains("YEARLY"))
                        YEARLY = param[i];
                    if (param[i].Contains("BYSETPOS"))
                    {
                        BYSETPOS = param[i];
                        BYSETPOSCOUNT = param[i + 1];
                    }
                    if (param[i].Contains("BYDAY"))
                    {
                        BYDAYPOSITION = i;
                        BYDAY = param[i];
                        BYDAYVALUE = param[i + 1];
                    }
                    if (param[i].Contains("BYMONTHDAY"))
                    {
                        BYMONTHDAYPOSITION = i;
                        BYMONTHDAY = param[i];
                        BYMONTHDAYCOUNT = param[i + 1];
                    }
                    if (param[i].Contains("BYMONTH"))
                    {
                        BYMONTH = param[i];
                        BYMONTHCOUNT = param[i + 1];
                    }
                }
            }

            if (EndDateFilter.HasValue && EndDateFilter.Value.Date < RecStartDate.Date)
                throw new ArgumentOutOfRangeException($"{nameof(EndDateFilter)} can't be less than {nameof(RecStartDate)}");
            if (StartDateFilter.HasValue && EndDateFilter.HasValue && StartDateFilter.Value > EndDateFilter.Value)
                throw new ArgumentOutOfRangeException($"{nameof(EndDateFilter)} can't be less than {nameof(StartDateFilter)}");

            var realStartDate = StartDateFilter ?? RecStartDate;
            var realEndDate = EndDateFilter ?? DateTime.MaxValue;

            var RecDateCollection = new ObservableCollection<DateTime>();

            if (RRule == null)
            {
                if (realStartDate <= RecStartDate && RecStartDate <= realEndDate)
                    RecDateCollection.Add(RecStartDate);
                RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                return RecDateCollection;
            }

            DateTime startDate = RecStartDate;
            var ruleSeperator = new[] { '=', ';', ',' };
            var weeklySeperator = new[] { ';' };
            string[] ruleArray = RRule.Split(ruleSeperator) ?? new string[]{};
            FindKeyIndex(ruleArray);
            string[] weeklyRule = RRule.Split(weeklySeperator) ?? new string[]{};
            FindWeeklyRule(weeklyRule);
            FindExdateList(weeklyRule);
            if (ruleArray.Length != 0 && RRule != "")
            {
                DateTime addDate = startDate;
                int currentCount = 0;
                int recCount = int.MaxValue;
                if (!string.IsNullOrEmpty(RECCOUNT))
                    int.TryParse(RECCOUNT, out recCount);

                #region DAILY

                if (DAILY == "DAILY")
                {
                    if ((ruleArray.Length > 4 && INTERVAL == "INTERVAL") || ruleArray.Length == 4)
                    {
                        int DyDayGap = ruleArray.Length == 4 ? 1 : int.Parse(INTERVALCOUNT);
                        for (int i = 0; i < recCount; i++)
                        {
                            if (addDate > realEndDate)
                            {
                                RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                return RecDateCollection;
                            }
                            if (realStartDate <= addDate)
                                RecDateCollection.Add(addDate);
                            currentCount++;
                            try
                            {
                                addDate = addDate.AddDays(DyDayGap);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                return RecDateCollection;
                            }
                        }
                    }
                    else if (ruleArray.Length > 4 && BYDAY == "BYDAY")
                    {
                        while (currentCount < recCount)
                        {
                            if (addDate.DayOfWeek != DayOfWeek.Sunday && addDate.DayOfWeek != DayOfWeek.Saturday)
                            {
                                if (addDate > realEndDate)
                                {
                                    RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                    return RecDateCollection;
                                }
                                if (realStartDate <= addDate)
                                    RecDateCollection.Add(addDate);
                                currentCount++;
                            }
                            try
                            {
                                addDate = addDate.AddDays(1);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                return RecDateCollection;
                            }
                        }
                    }
                }

                #endregion

                #region WEEKLY

                else if (WEEKLY == "WEEKLY")
                {
                    int WyWeekGap = ruleArray.Length > 4 && INTERVAL == "INTERVAL" ? int.Parse(INTERVALCOUNT) : 1;
                    bool isweeklyselected = weeklyRule[WEEKLYBYDAYPOS].Length > 6;
                    while (currentCount < recCount && isweeklyselected)
                    {
                        switch (addDate.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                {
                                    if (weeklyRule[WEEKLYBYDAYPOS].Contains("SU") && currentCount < recCount)
                                    {
                                        if (addDate > realEndDate)
                                        {
                                            RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                            return RecDateCollection;
                                        }
                                        if (realStartDate <= addDate)
                                            RecDateCollection.Add(addDate);
                                        currentCount++;
                                    }
                                    break;
                                }
                            case DayOfWeek.Monday:
                                {
                                    if (weeklyRule[WEEKLYBYDAYPOS].Contains("MO") && currentCount < recCount)
                                    {
                                        if (addDate > realEndDate)
                                        {
                                            RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                            return RecDateCollection;
                                        }
                                        if (realStartDate <= addDate)
                                            RecDateCollection.Add(addDate);
                                        currentCount++;
                                    }
                                    break;
                                }
                            case DayOfWeek.Tuesday:
                                {
                                    if (weeklyRule[WEEKLYBYDAYPOS].Contains("TU") && currentCount < recCount)
                                    {
                                        if (addDate > realEndDate)
                                        {
                                            RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                            return RecDateCollection;
                                        }
                                        if (realStartDate <= addDate)
                                            RecDateCollection.Add(addDate);
                                        currentCount++;
                                    }
                                    break;
                                }
                            case DayOfWeek.Wednesday:
                                {
                                    if (weeklyRule[WEEKLYBYDAYPOS].Contains("WE") && currentCount < recCount)
                                    {
                                        if (addDate > realEndDate)
                                        {
                                            RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                            return RecDateCollection;
                                        }
                                        if (realStartDate <= addDate)
                                            RecDateCollection.Add(addDate);
                                        currentCount++;
                                    }
                                    break;
                                }
                            case DayOfWeek.Thursday:
                                {
                                    if (weeklyRule[WEEKLYBYDAYPOS].Contains("TH") && currentCount < recCount)
                                    {
                                        if (addDate > realEndDate)
                                        {
                                            RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                            return RecDateCollection;
                                        }
                                        if (realStartDate <= addDate)
                                            RecDateCollection.Add(addDate);
                                        currentCount++;
                                    }
                                    break;
                                }
                            case DayOfWeek.Friday:
                                {
                                    if (weeklyRule[WEEKLYBYDAYPOS].Contains("FR") && currentCount < recCount)
                                    {
                                        if (addDate > realEndDate)
                                        {
                                            RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                            return RecDateCollection;
                                        }
                                        if (realStartDate <= addDate)
                                            RecDateCollection.Add(addDate);
                                        currentCount++;
                                    }
                                    break;
                                }
                            case DayOfWeek.Saturday:
                                {
                                    if (weeklyRule[WEEKLYBYDAYPOS].Contains("SA") && currentCount < recCount)
                                    {
                                        if (addDate > realEndDate)
                                        {
                                            RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                            return RecDateCollection;
                                        }
                                        if (realStartDate <= addDate)
                                            RecDateCollection.Add(addDate);
                                        currentCount++;
                                    }
                                    break;
                                }
                        }
                        try
                        {
                            addDate = addDate.DayOfWeek == DayOfWeek.Saturday ? addDate.AddDays(((WyWeekGap - 1) * 7) + 1) : addDate.AddDays(1);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                            return RecDateCollection;
                        }
                    }
                }

                #endregion

                #region MONTHLY

                else if (MONTHLY == "MONTHLY")
                {
                    int MyMonthGap = ruleArray.Length > 4 && INTERVAL == "INTERVAL" ? int.Parse(INTERVALCOUNT) : 1;
                    int position = ruleArray.Length > 4 && INTERVAL == "INTERVAL" ? 6 : BYMONTHDAYPOSITION;
                    if (BYMONTHDAY == "BYMONTHDAY")
                    {
                        int monthDate = int.Parse(BYMONTHDAYCOUNT);
                        if (monthDate < 30)
                        {
                            int currDate = int.Parse(startDate.Day.ToString());
                            var temp = new DateTime(addDate.Year, addDate.Month, monthDate, RecStartDate.Hour, RecStartDate.Minute, realStartDate.Second);
                            try
                            {
                                addDate = monthDate < currDate ? temp.AddMonths(1) : temp;
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                return RecDateCollection;
                            }
                            for (int i = 0; i < recCount; i++)
                            {
                                if (addDate.Month == 2 && monthDate > 28)
                                {
                                    addDate = new DateTime(addDate.Year, addDate.Month, DateTime.DaysInMonth(addDate.Year, 2), RecStartDate.Hour, RecStartDate.Minute, realStartDate.Second);
                                    if (addDate > realEndDate)
                                    {
                                        RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                        return RecDateCollection;
                                    }
                                    if (realStartDate <= addDate)
                                        RecDateCollection.Add(addDate);
                                    currentCount++;
                                    try
                                    {
                                        addDate = addDate.AddMonths(MyMonthGap);
                                    }
                                    catch (ArgumentOutOfRangeException)
                                    {
                                        RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                        return RecDateCollection;
                                    }
                                    addDate = new DateTime(addDate.Year, addDate.Month, monthDate, RecStartDate.Hour, RecStartDate.Minute, realStartDate.Second);
                                }
                                else
                                {
                                    if (addDate > realEndDate)
                                    {
                                        RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                        return RecDateCollection;
                                    }
                                    if (realStartDate <= addDate)
                                        RecDateCollection.Add(addDate);
                                    currentCount++;
                                    try
                                    {
                                        addDate = addDate.AddMonths(MyMonthGap);
                                    }
                                    catch (ArgumentOutOfRangeException)
                                    {
                                        RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                        return RecDateCollection;
                                    }
                                }
                            }
                        }
                        else
                        {
                            addDate = new DateTime(addDate.Year, addDate.Month, DateTime.DaysInMonth(addDate.Year, addDate.Month), RecStartDate.Hour, RecStartDate.Minute, realStartDate.Second);
                            for (int i = 0; i < recCount; i++)
                            {
                                if (addDate > realEndDate)
                                {
                                    RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                    return RecDateCollection;
                                }
                                if (realStartDate <= addDate)
                                    RecDateCollection.Add(addDate);
                                currentCount++;
                                try
                                {
                                    addDate = addDate.AddMonths(MyMonthGap);
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                    return RecDateCollection;
                                }
                                addDate = new DateTime(addDate.Year, addDate.Month, DateTime.DaysInMonth(addDate.Year, addDate.Month), RecStartDate.Hour, RecStartDate.Minute, realStartDate.Second);
                            }
                        }
                    }
                    else if (BYDAY == "BYDAY")
                    {
                        while (currentCount < recCount)
                        {
                            var monthStart = new DateTime(addDate.Year, addDate.Month, 1, RecStartDate.Hour, RecStartDate.Minute, realStartDate.Second);
                            DateTime weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = GetWeekDay(BYDAYVALUE) - 1;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                                nthWeek = int.Parse(BYSETPOSCOUNT) - 1;
                            else
                                nthWeek = int.Parse(BYSETPOSCOUNT);
                            try
                            {
                                addDate = weekStartDate.AddDays((nthWeek) * 7);
                                addDate = addDate.AddDays(nthweekDay);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                return RecDateCollection;
                            }
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                try
                                {
                                    addDate = addDate.AddMonths(1);
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                    return RecDateCollection;
                                }
                                continue;
                            }

                            if (addDate > realEndDate)
                            {
                                RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                return RecDateCollection;
                            }
                            if (realStartDate <= addDate)
                                RecDateCollection.Add(addDate);
                            currentCount++;
                            try
                            {
                                addDate = addDate.AddMonths(MyMonthGap);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                return RecDateCollection;
                            }
                        }
                    }
                }

                #endregion

                #region YEARLY

                else if (YEARLY == "YEARLY")
                {
                    int YyYearGap = ruleArray.Length > 4 && INTERVAL == "INTERVAL" ? int.Parse(INTERVALCOUNT) : 1;
                    int position = ruleArray.Length > 4 && INTERVAL == "INTERVAL" ? 6 : BYMONTHDAYPOSITION;
                    if (BYMONTHDAY == "BYMONTHDAY")
                    {
                        int monthIndex = int.Parse(BYMONTHCOUNT);
                        int dayIndex = int.Parse(BYMONTHDAYCOUNT);
                        if (monthIndex > 0 && monthIndex <= 12)
                        {
                            int bound = DateTime.DaysInMonth(addDate.Year, monthIndex);
                            if (bound >= dayIndex)
                            {
                                var specificDate = new DateTime(addDate.Year, monthIndex, dayIndex, RecStartDate.Hour, RecStartDate.Minute, realStartDate.Second);
                                if (specificDate.Date < addDate)
                                {
                                    addDate = specificDate;
                                    try
                                    {
                                        addDate = addDate.AddYears(1);
                                    }
                                    catch (ArgumentOutOfRangeException)
                                    {
                                        RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                        return RecDateCollection;
                                    }
                                }
                                else
                                    addDate = specificDate;

                                for (int i = 0; i < recCount; i++)
                                {
                                    if (addDate > realEndDate)
                                    {
                                        RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                        return RecDateCollection;
                                    }
                                    if (realStartDate <= addDate)
                                        RecDateCollection.Add(addDate);
                                    currentCount++;
                                    try
                                    {
                                        addDate = addDate.AddYears(YyYearGap);
                                    }
                                    catch (ArgumentOutOfRangeException)
                                    {
                                        RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                        return RecDateCollection;
                                    }
                                }
                            }
                        }
                    }
                    else if (BYDAY == "BYDAY")
                    {
                        int monthIndex = int.Parse(BYMONTHCOUNT);
                        while (currentCount < recCount)
                        {
                            var monthStart = new DateTime(addDate.Year, monthIndex, 1, RecStartDate.Hour, RecStartDate.Minute, realStartDate.Second);
                            DateTime weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = GetWeekDay(BYDAYVALUE) - 1;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                                nthWeek = int.Parse(BYSETPOSCOUNT) - 1;
                            else
                                nthWeek = int.Parse(BYSETPOSCOUNT);
                            try
                            {
                                addDate = weekStartDate.AddDays((nthWeek) * 7);
                                addDate = addDate.AddDays(nthweekDay);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                return RecDateCollection;
                            }
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                try
                                {
                                    addDate = addDate.AddYears(1);
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                    return RecDateCollection;
                                }
                                continue;
                            }

                            if (addDate > realEndDate)
                            {
                                RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                return RecDateCollection;
                            }
                            if (realStartDate <= addDate)
                                RecDateCollection.Add(addDate);
                            currentCount++;
                            try
                            {
                                addDate = addDate.AddYears(YyYearGap);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
                                return RecDateCollection;
                            }
                        }
                    }
                }

                #endregion
            }
            RecDateCollection.RemoveWhere(_ => exDateList.Any(d => d.Date == _.Date));
            return RecDateCollection;
        }

        public static IEnumerable<TimeSlotUnit> GetRecurrenceTimeSlotCollection(this IEnumerable<DateTime> recurrences, Timeslot timeSlot)
        {
            var timeslotCollection = new ObservableCollection<TimeSlotUnit>();

            foreach (var recurrence in recurrences)
            {
                var start = new DateTime(recurrence.Year, recurrence.Month, recurrence.Day, timeSlot.StartTime.Hours, timeSlot.StartTime.Minutes, timeSlot.StartTime.Seconds);
                var end = new DateTime(recurrence.Year, recurrence.Month, recurrence.Day, timeSlot.EndTime.Hours, timeSlot.EndTime.Minutes, timeSlot.EndTime.Seconds);
                if (timeSlot.EndTime <= timeSlot.StartTime)
                    end = end.AddDays(1);
                timeslotCollection.Add(new TimeSlotUnit(start, end));
            }

            return timeslotCollection;
        }

        public static TimeSlotUnit Contains(this IEnumerable<TimeSlotUnit> recurrences, DateTime dateTime) =>
            recurrences.SingleOrDefault(_ => _.StartDateTime <= dateTime && dateTime < _.EndDateTime);
    }

    public class TimeSlotUnit
    {
        public DateTime StartDateTime { get; private set; }

        public DateTime EndDateTime { get; private set; }

        public TimeSlotUnit(DateTime start, DateTime end)
        {
            StartDateTime = start;
            EndDateTime = end;
        }
    }
}
