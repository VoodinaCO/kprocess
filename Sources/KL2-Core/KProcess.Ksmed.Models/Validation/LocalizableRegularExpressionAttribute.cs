using KProcess.Globalization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KProcess.Ksmed.Models.Validation
{
    /// <summary>
    /// Représente un <see cref="RegularExpressionAttribute" /> localisable.
    /// </summary>
    public class LocalizableRegularExpressionAttribute : RegularExpressionAttribute
    {
        public IEnumerable<object> Params { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="LocalizableRegularExpressionAttribute"/>.
        /// </summary>
        /// <param name="pattern">L'expression régulière.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="pattern"/> est null.</exception>
        public LocalizableRegularExpressionAttribute(string pattern)
            :base(pattern)
        {
        }

        /// <summary>
        /// Formatte le message d'erreur affiché.
        /// </summary>
        /// <param name="name">Le nom du champ où la validation a échoué.</param>
        /// <returns>
        /// Le message formatté.
        /// </returns>
        public override string FormatErrorMessage(string name) =>
            LocalizationManager.GetStringFormat(ErrorMessageResourceName, Params);
    }
}
