using KProcess.Ksmed.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.TabletClient.Converter
{
    /// <summary>
    /// Convertit des ticks en chaîne de caractères.
    /// </summary>
    public class TicksToStringConverter : MarkupExtension, IMultiValueConverter
    {
        /// <summary>
        /// Convertitune la valeur.
        /// </summary>
        /// <param name="values">Les valeurs produites par les sources du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>
        /// Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2 || values.Any(_ => _ == null) || values.Any(_ => _ == DependencyProperty.UnsetValue))
                return null;
            try
            {
                long ticks = System.Convert.ToInt64(values[0]);
                CurrentTimeScale = System.Convert.ToInt64(values[1]);

                return TicksToString(ticks);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Non supporté.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetTypes">Les types des propriétés cibles du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>
        /// Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { ParseToTicks((string)value), 1000000};
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        /// <summary>
        /// Convertit des ticks en string.
        /// </summary>
        /// <param name="ticks">les ticks.</param>
        /// <returns>La chaîne formattée.</returns>
        string TicksToString(long ticks, long timeScale = -1)
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
        /// Parse une chaîne et la convertit en ticks.
        /// </summary>
        /// <param name="input">La chaîne.</param>
        /// <returns>Les ticks, ou 0 si le contenu est invalide.</returns>
        long ParseToTicks(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0L;

            var parts = input.Split(':');

            if (parts.Length == 3)
            {
                if (int.TryParse(parts[0], out int hours))
                {
                    var strWithoutHours = string.Join(":", "00", parts[1], parts[2]);
                    if (TimeSpan.TryParse(strWithoutHours, out TimeSpan tsWithoutHours))
                        return (TimeSpan.FromHours(hours) + tsWithoutHours).Ticks;
                    return 0L;
                }
            }

            // Si tout le reste échoue...
            if (TimeSpan.TryParse(input, out TimeSpan ts))
                return ts.Ticks;
            return 0L;
        }

        /// <summary>
        /// Effectue un arrondi sur les ticks.
        /// </summary>
        /// <param name="ticks">Les ticks.</param>
        /// <param name="scale">L'arrondi.</param>
        /// <returns>Les ticks arrondis.</returns>
        long RoundTime(long ticks, long scale)
        {
            return System.Convert.ToInt64(Math.Round(ticks / (decimal)scale)) * scale;
        }

        /// <summary>
        /// Obtient l'échelle de temps du projet en cours.
        /// </summary>
        long CurrentTimeScale { get; set; }
    }

    /// <summary>
    /// Convertit des ticks en chaîne de caractères.
    /// </summary>
    public class LinkedTicksToStringConverter : MarkupExtension, IMultiValueConverter
    {
        /// <summary>
        /// Convertitune la valeur.
        /// </summary>
        /// <param name="values">Les valeurs produites par les sources du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>
        /// Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2 || values.Any(_ => _ == null) || values.Any(_ => _ == DependencyProperty.UnsetValue))
                return null;
            try
            {
                long ticks = System.Convert.ToInt64(values[0]);
                CurrentTimeScale = System.Convert.ToInt64((values[1] as TrackableCollection<PublicationLocalization>)?.FirstOrDefault()?.Publication?.TimeScale ?? -1);

                return TicksToString(ticks);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Non supporté.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetTypes">Les types des propriétés cibles du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>
        /// Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { ParseToTicks((string)value), 1000000 };
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        /// <summary>
        /// Convertit des ticks en string.
        /// </summary>
        /// <param name="ticks">les ticks.</param>
        /// <returns>La chaîne formattée.</returns>
        string TicksToString(long ticks, long timeScale = -1)
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
        /// Parse une chaîne et la convertit en ticks.
        /// </summary>
        /// <param name="input">La chaîne.</param>
        /// <returns>Les ticks, ou 0 si le contenu est invalide.</returns>
        long ParseToTicks(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0L;

            var parts = input.Split(':');

            if (parts.Length == 3)
            {
                if (int.TryParse(parts[0], out int hours))
                {
                    var strWithoutHours = string.Join(":", "00", parts[1], parts[2]);
                    if (TimeSpan.TryParse(strWithoutHours, out TimeSpan tsWithoutHours))
                        return (TimeSpan.FromHours(hours) + tsWithoutHours).Ticks;
                    return 0L;
                }
            }

            // Si tout le reste échoue...
            if (TimeSpan.TryParse(input, out TimeSpan ts))
                return ts.Ticks;
            return 0L;
        }

        /// <summary>
        /// Effectue un arrondi sur les ticks.
        /// </summary>
        /// <param name="ticks">Les ticks.</param>
        /// <param name="scale">L'arrondi.</param>
        /// <returns>Les ticks arrondis.</returns>
        long RoundTime(long ticks, long scale)
        {
            return System.Convert.ToInt64(Math.Round(ticks / (decimal)scale)) * scale;
        }

        /// <summary>
        /// Obtient l'échelle de temps du projet en cours.
        /// </summary>
        long CurrentTimeScale { get; set; }
    }
}
