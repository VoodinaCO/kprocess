using System;
using System.Text;

namespace KProcess.KL2.WebAdmin.Utils
{
    public class TicksUtil
    {
        /// <summary>
        /// Convertit des ticks en string.
        /// </summary>
        /// <param name="ticks">les ticks.</param>
        /// <returns>La chaîne formattée.</returns>
        public static string TicksToString(long ticks, long timeScale = 1)
        {
            // arrondir les ticks
            ticks = RoundTime(ticks, timeScale);

            var timeSpan = TimeSpan.FromTicks(ticks);

            var sb = new StringBuilder();

            if (ticks < 0)
                sb.Append("-");

            sb.AppendFormat("{0:00}:", Math.Floor(timeSpan.TotalHours));
            sb.AppendFormat("{0:00}:", timeSpan.Minutes);
            sb.AppendFormat("{0:00}", timeSpan.Seconds);

            string format = "";

            if (timeScale < TimeSpan.FromSeconds(1).Ticks)
                format += "\\.f";

            if (timeScale < TimeSpan.FromSeconds(.1).Ticks)
                format += "f";

            if (timeScale < TimeSpan.FromSeconds(.01).Ticks)
                format += "f";

            if (timeScale < TimeSpan.FromSeconds(.001).Ticks)
                format += "f";

            if (format.Length > 0)
            {
                sb.Append(timeSpan.ToString(format));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Effectue un arrondi sur les ticks.
        /// </summary>
        /// <param name="ticks">Les ticks.</param>
        /// <param name="scale">L'arrondi.</param>
        /// <returns>Les ticks arrondis.</returns>
        private static long RoundTime(long ticks, long scale)
        {
            return Convert.ToInt64(Math.Round(ticks / (decimal)scale)) * scale;
        }

    }
}