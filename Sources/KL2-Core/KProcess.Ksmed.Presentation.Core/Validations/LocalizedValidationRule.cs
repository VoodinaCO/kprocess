using KProcess.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KProcess.Ksmed.Presentation.Core.Validations
{
    public class StringToIntValidationRule : ValidationRule
    {
        private string _errorMessageKey;
        public string ErrorMessageKey
        {
            get { return _errorMessageKey; }
            set { _errorMessageKey = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int i;
            if (int.TryParse(value.ToString(), out i))
                return new ValidationResult(true, null);

            return new ValidationResult(false, LocalizationManager.GetStringFormat(ErrorMessageKey, value));
        }
    }
}
