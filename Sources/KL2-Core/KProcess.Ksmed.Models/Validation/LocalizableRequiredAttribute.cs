using KProcess.Globalization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KProcess.Ksmed.Models.Validation
{
    /// <summary>
    /// Represente un <see cref="RequiredAttribute"/> localisable.
    /// </summary>
    public class LocalizableRequiredAttribute : RequiredAttribute
    {
        public IEnumerable<object> Params { get; set; }

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
