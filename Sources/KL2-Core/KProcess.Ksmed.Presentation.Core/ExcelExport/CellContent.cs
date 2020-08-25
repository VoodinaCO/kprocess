#define USE1904DATESYSTEM
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Core.ExcelExport
{

    /// <summary>
    /// Représente le contenu d'une cellule à écrire.
    /// </summary>
    public class CellContent
    {
#if USE1904DATESYSTEM
        private const bool Use1904DateSystem = true;
        private const int Date1904Offset = -1462;
#endif

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="value">La chaîne de caractères.</param>
        public CellContent(string value)
            : this(value, CellDataType.String)
        {
        }

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="value">La valeur.</param>
        /// <param name="dataType">Le type de la valeur.</param>
        public CellContent(string value, CellDataType dataType)
        {
            this.Value = value;
            this.DataType = dataType;
        }

        /// <summary>
        /// Obtient ou définit la valeur sous forme de chaîne de caractères.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Obtient ou définit le type de données.
        /// </summary>
        public CellDataType DataType { get; set; }

        /// <summary>
        /// Obtient ou définit le formattage particulier.
        /// </summary>
        public string CustomNumberFormat { get; set; }

        /// <summary>
        /// Convertit implicitement une <see cref="string"/> en <see cref="CellContent"/>.
        /// </summary>
        /// <param name="value">La valeur à convertir.</param>
        /// <returns>Le <see cref="CellContent"/> créé.</returns>
        public static implicit operator CellContent(string value)
        {
            return new CellContent(value, CellDataType.String);
        }

        /// <summary>
        /// Convertit implicitement une <see cref="double"/> en <see cref="CellContent"/>.
        /// </summary>
        /// <param name="value">La valeur à convertir.</param>
        /// <returns>Le <see cref="CellContent"/> créé.</returns>
        public static implicit operator CellContent(double value)
        {
            return new CellContent(value.ToString(CultureInfo.InvariantCulture), CellDataType.Number);
        }

        /// <summary>
        /// Convertit implicitement une <see cref="decimal"/> en <see cref="CellContent"/>.
        /// </summary>
        /// <param name="value">La valeur à convertir.</param>
        /// <returns>Le <see cref="CellContent"/> créé.</returns>
        public static implicit operator CellContent(decimal value)
        {
            return new CellContent(value.ToString(CultureInfo.InvariantCulture), CellDataType.Number);
        }

        /// <summary>
        /// Convertit implicitement une <see cref="int"/> en <see cref="CellContent"/>.
        /// </summary>
        /// <param name="value">La valeur à convertir.</param>
        /// <returns>Le <see cref="CellContent"/> créé.</returns>
        public static implicit operator CellContent(int value)
        {
            return new CellContent(value.ToString(CultureInfo.InvariantCulture), CellDataType.Number);
        }

        /// <summary>
        /// Convertit implicitement une <see cref="DateTime"/> en <see cref="CellContent"/>.
        /// </summary>
        /// <param name="value">La valeur à convertir.</param>
        /// <returns>Le <see cref="CellContent"/> créé.</returns>
        public static implicit operator CellContent(DateTime value)
        {
#if USE1904DATESYSTEM
            var days = value.ToOADate() + Date1904Offset; // Excel gérera mal les dates < à 1904. Bug connu.
            return new CellContent(days.ToString(CultureInfo.InvariantCulture), CellDataType.Date);
#else
            return new CellContent(value.ToOADate().ToString(CultureInfo.InvariantCulture), CellDataType.Date);
#endif
        }

        /// <summary>
        /// Convertit implicitement une <see cref="TimeSpan"/> en <see cref="CellContent"/>.
        /// </summary>
        /// <param name="value">La valeur à convertir.</param>
        /// <returns>Le <see cref="CellContent"/> créé.</returns>
        public static implicit operator CellContent(TimeSpan value)
        {
            return new CellContent(value.TotalDays.ToString(System.Globalization.CultureInfo.InvariantCulture), CellDataType.TimeSpan);
        }

        /// <summary>
        /// Crée un <see cref="CellContent"/> formatté en pourcentage à partir d'un nombre.
        /// </summary>
        /// <param name="value">Le nombre</param>
        /// <returns>Le <see cref="CellContent"/> créé.</returns>
        public static CellContent TimeSpan(long ticks)
        {
            return System.TimeSpan.FromTicks(ticks);
        }

        /// <summary>
        /// Crée un <see cref="CellContent"/> formatté en pourcentage à partir d'un nombre.
        /// </summary>
        /// <param name="value">Le nombre</param>
        /// <returns>Le <see cref="CellContent"/> créé.</returns>
        public static CellContent Percentage(double value)
        {
            return new CellContent(value.ToString(CultureInfo.InvariantCulture), CellDataType.Percentage);
        }
    }
}
