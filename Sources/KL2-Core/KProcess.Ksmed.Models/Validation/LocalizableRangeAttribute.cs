using KProcess.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KProcess.Ksmed.Models.Validation
{
    /// <summary>
    /// Représente un <see cref="RangeAttribute"/> localisable.
    /// </summary>
    /// 
    [Serializable]
    public class LocalizableRangeAttribute : RangeAttribute
    {
        public IEnumerable<object> Params { get; set; }

        public LocalizableRangeAttribute(Type type, string minimum, string maximum)
            : base(type, minimum, maximum)
        { }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="LocalizableRangeAttribute"/>.
        /// </summary>
        /// <param name="minimum">Le minimum.</param>
        /// <param name="maximum">Le maximum.</param>
        public LocalizableRangeAttribute(double minimum, double maximum)
            : base(minimum, maximum)
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="LocalizableRangeAttribute"/>.
        /// </summary>
        /// <param name="minimum">Le minimum.</param>
        /// <param name="maximum">Le maximum.</param>
        public LocalizableRangeAttribute(int minimum, int maximum)
            : base(minimum, maximum)
        {
        }

        /// <summary>
        /// Formatte le message d'erreur affiché.
        /// </summary>
        /// <param name="name">Le nom du champ où la validation a échoué.</param>
        /// <returns>
        /// Le message formatté.
        /// </returns>
        public override string FormatErrorMessage(string name)
        {
            object[] finalParams = { Minimum, Maximum };
            finalParams.AddRange(Params);
            return LocalizationManager.GetStringFormat(ErrorMessageResourceName, finalParams);
        }
    }
}
