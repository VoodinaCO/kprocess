using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using KProcess.Ksmed.Business;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit le comportement d'un service permettant de formatter un temps ou une durée.
    /// </summary>
    public class TimeTicksFormatService : ITimeTicksFormatService
    {
        private static IServiceBus _serviceBus;

        /// <summary>
        /// Obtient l'échelle de temps du projet en cours.
        /// </summary>
        public long CurrentTimeScale
        {
            get
            {
                if (DesignMode.IsInDesignMode)
                    return KnownTimeScales.Second;

                if (_serviceBus == null)
                    _serviceBus = IoC.Resolve<IServiceBus>();

                var currentProject = _serviceBus.Get<IProjectManagerService>().CurrentProject;
                if (currentProject != null)
                    return currentProject.TimeScale;
                else
                    return KnownTimeScales.Second;
            }
        }

        /// <summary>
        /// Convertit des ticks en string.
        /// </summary>
        /// <param name="ticks">les ticks.</param>
        /// <returns>La chaîne formattée.</returns>
        public string TicksToString(long ticks, long timeScale = -1)
        {
            if (timeScale == -1)
                timeScale = CurrentTimeScale;

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
        public long RoundTime(long ticks, long scale)
        {
            return Convert.ToInt64(Math.Round(ticks / (decimal)scale)) * scale;
        }

        private MediaPlayerTimeConverter _currentPositionConverter;
        /// <summary>
        /// Obtient le convertisseur de position courante
        /// </summary>
        public IMultiValueConverter CurrentPositionConverter
        {
            get
            {
                if (_currentPositionConverter == null)
                    _currentPositionConverter = new MediaPlayerTimeConverter(this);
                return _currentPositionConverter;
            }
        }

        /// <summary>
        /// Formatte un temps.
        /// </summary>
        /// <param name="ticks">Des ticks.</param>
        /// <returns>
        /// Le temps formatté
        /// </returns>
        public string FormatTime(long ticks)
        {
            return TicksToString(ticks);
        }

        /// <summary>
        /// Parse une chaîne et la convertit en ticks.
        /// </summary>
        /// <param name="input">La chaîne.</param>
        /// <returns>Les ticks, ou 0 si le contenu est invalide.</returns>
        public long ParseToTicks(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0L;

            var parts = input.Split(':');

            if (parts.Length == 3)
            {
                int hours;
                if (int.TryParse(parts[0], out hours))
                {
                    var strWithoutHours = string.Join(":", "00", parts[1], parts[2]);
                    TimeSpan tsWithoutHours;
                    if (TimeSpan.TryParse(strWithoutHours, out tsWithoutHours))
                        return (TimeSpan.FromHours(hours) + tsWithoutHours).Ticks;
                    else
                        return 0L;
                }
            }

            // Si tout le reste échoue...
            TimeSpan ts;
            if (TimeSpan.TryParse(input, out ts))
                return ts.Ticks;
            else
                return 0L;
        }

        private class MediaPlayerTimeConverter : IMultiValueConverter
        {
            private ITimeTicksFormatService _service;
            public MediaPlayerTimeConverter(ITimeTicksFormatService service)
            {
                _service = service;
            }

            #region IMultiValueConverter Members

            public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                var position = (long)values[0];
                var duration = (long)values[1];

                return string.Format("{0} / {1}", _service.TicksToString(position), _service.TicksToString(duration));
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotSupportedException();
            }

            #endregion
        }
    }
}
