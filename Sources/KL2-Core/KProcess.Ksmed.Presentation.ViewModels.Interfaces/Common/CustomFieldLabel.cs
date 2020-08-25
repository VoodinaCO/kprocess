using KProcess.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le libellé d'un champ libre.
    /// </summary>
    public class CustomFieldLabel
    {
        private static Dictionary<int, string> _numericFallbackLabels = new Dictionary<int, string>();
        private static Dictionary<int, string> _textFallbackLabels = new Dictionary<int, string>();
        private const string FallbackDescriptionLocalizationKeyFormat = "View_Common_CustomFieldDefaultLabel_{0}{1}";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="isNumeric"><c>true</c> s'il s'agit d'un champ numérique, <c>false</c> s'il s'agit d'un champ texte.</param>
        /// <param name="index">L'Index (numéro) du libellé.</param>
        /// <param name="label">La valeur du libellé.</param>
        public CustomFieldLabel(bool isNumeric, int index, string label)
        {
            this.IsNumeric = isNumeric;
            this.Index = index;
            this.Label = label;
        }

        /// <summary>
        /// Obtient ou définit le libellé.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Obtient une valeur indiquant si le libellé est défini.
        /// </summary>
        public bool IsDefined
        {
            get { return Label != null; }
        }

        /// <summary>
        /// Obtient le libellé, ou, si celui-ci est invalide, une valeur de repli décrivant le libellé.
        /// </summary>
        public string LabelOrFallbackDescription
        {
            get { return IsDefined ? Label : GetFallbackDescription(IsNumeric, Index); }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le libellé est pour une valeur numérique.
        /// </summary>
        public bool IsNumeric { get; private set; }

        /// <summary>
        /// Obtient une valeur indiquant l'index (numéro) du libellé.
        /// </summary>
        public int Index { get; private set; }

        private static string GetFallbackDescription(bool numeric, int index)
        {
            Dictionary<int, string> cache;

            if (numeric)
                cache = _numericFallbackLabels;
            else
                cache = _textFallbackLabels;

            if (cache.ContainsKey(index))
                return cache[index];
            else
            {
                var localizationKeyFieldtype = numeric ? "Numeric" : "Text";
                var localizationKey = string.Format(FallbackDescriptionLocalizationKeyFormat, localizationKeyFieldtype, index);

                var value = LocalizationManager.GetString(localizationKey);

                cache[index] = value;

                return value;
            }
        }

    }
}
