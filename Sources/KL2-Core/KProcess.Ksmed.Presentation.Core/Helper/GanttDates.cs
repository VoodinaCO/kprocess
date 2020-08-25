using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Fournit des méthodes et propriétés d'aide à la gestion et à la conversion des dates dans les Gantt.
    /// </summary>
    public static class GanttDates
    {

        private static readonly DateTime _startDate;

        /// <summary>
        /// Initialise la classe <see cref="GanttDates"/>.
        /// </summary>
        static GanttDates()
        {
            _startDate = DateTime.Today;
        }

        /// <summary>
        /// Obtient la date de début de toutes les dates des Gantt.
        /// </summary>
        public static DateTime StartDate
        {
            get { return _startDate; }
        }

        /// <summary>
        /// Obtient la date de fin par défaut.
        /// </summary>
        public static DateTime DefaultEndDate
        {
            get { return StartDate.AddHours(1); }
        }

        /// <summary>
        /// Convertit des ticks en date de début.
        /// </summary>
        /// <param name="ticks">Les ticks.</param>
        /// <returns>La date de début.</returns>
        public static DateTime ToDateTime(long ticks)
        {
            return StartDate.AddTicks(ticks);
        }

        /// <summary>
        /// Convertit des ticks en durée (TimeSpan).
        /// </summary>
        /// <param name="ticks">Les ticks.</param>
        /// <returns>La durée.</returns>
        public static TimeSpan ToTimeSpan(long ticks)
        {
            return TimeSpan.FromTicks(ticks);
        }

        /// <summary>
        /// Convertit une date en ticks.
        /// </summary>
        /// <param name="dateTime">La date.</param>
        /// <returns>Les ticks.</returns>
        public static long ToTicks(DateTime dateTime)
        {
            return (dateTime - StartDate).Ticks;
        }

        /// <summary>
        /// Convertit une durée en ticks.
        /// </summary>
        /// <param name="timeSpan">La durée.</param>
        /// <returns>Les ticks.</returns>
        public static long ToTicks(TimeSpan timeSpan)
        {
            return timeSpan.Ticks;
        }

    }
}
