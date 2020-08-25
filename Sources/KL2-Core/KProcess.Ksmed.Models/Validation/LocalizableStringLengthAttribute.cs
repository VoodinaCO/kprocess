using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.Collections.Generic;

namespace KProcess.Ksmed.Models.Validation
{
    /// <summary>
    /// Represente une chaîne de caractère localisable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class LocalizableStringLengthAttribute : ValueValidatorAttribute
    {
        readonly int lowerBound;
        readonly RangeBoundaryType lowerBoundType;
        readonly int upperBound;
        readonly RangeBoundaryType upperBoundType;

        public IEnumerable<object> Params { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="LocalizableStringLengthAttribute"/>.
        /// </summary>
        /// <param name="upperBound">La limite maximale.</param>
        public LocalizableStringLengthAttribute(int upperBound)
            : this(0, RangeBoundaryType.Ignore, upperBound, RangeBoundaryType.Inclusive)
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="LocalizableStringLengthAttribute"/>.
        /// </summary>
        /// <param name="lowerBound">La limite minimale.</param>
        /// <param name="lowerBoundType">Le type de la limite.</param>
        /// <param name="upperBound">La limite maximale.</param>
        /// <param name="upperBoundType">Le type de la limite.</param>
        public LocalizableStringLengthAttribute(int lowerBound, RangeBoundaryType lowerBoundType, int upperBound, RangeBoundaryType upperBoundType)
        {
            this.lowerBound = lowerBound;
            this.lowerBoundType = lowerBoundType;
            this.upperBound = upperBound;
            this.upperBoundType = upperBoundType;
        }

        /// <summary>
        /// Crée le validateur.
        /// </summary>
        /// <param name="targetType">Le type de l'objet qui va être validé.</param>
        /// <returns>
        /// Le validateur créé.
        /// </returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            MessageTemplate = ErrorMessageResourceName;
            var validator = new LocalizableStringLengthValidator(lowerBound, lowerBoundType, upperBound, upperBoundType, Negated)
            {
                Params = Params
            };
            return validator;
        }
    }
}
