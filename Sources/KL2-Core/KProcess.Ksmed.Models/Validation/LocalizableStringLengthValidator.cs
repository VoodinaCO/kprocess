using KProcess.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System.Collections.Generic;

namespace KProcess.Ksmed.Models.Validation
{
    /// <summary>
    /// Représente un validateur de taille de chaîne localisé
    /// </summary>
    public class LocalizableStringLengthValidator : StringLengthValidator
    {
        public IEnumerable<object> Params { get; set; }

        public LocalizableStringLengthValidator(int lowerBound, RangeBoundaryType lowerBoundType, int upperBound, RangeBoundaryType upperBoundType, bool negated)
            : base(lowerBound, lowerBoundType, upperBound, upperBoundType, negated)
        {
        }

        protected override string GetMessage(object objectToValidate, string key)
        {
            object[] finalParams = { UpperBound, LowerBound };
            finalParams.AddRange(Params);
            return LocalizationManager.GetStringFormat(MessageTemplate, finalParams);
        }

        protected override void DoValidate(string objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            if (objectToValidate == null)
                return;
            base.DoValidate(objectToValidate, currentTarget, key, validationResults);
        }
    }
}
