using System.Globalization;

namespace KProcess.Ksmed.Models
{
    public static class ValidateValueHelper
    {
        public static bool TryParse(string input, CultureInfo culture, out decimal? nullableOutput)
        {
            if (input == null)
            {
                nullableOutput = null;
                return true;
            }

            var stringValue = input
                .Replace(".", culture.NumberFormat.NumberDecimalSeparator)
                .Replace(",", culture.NumberFormat.NumberDecimalSeparator);
            var result = decimal.TryParse(stringValue, NumberStyles.Any, culture, out var output);
            nullableOutput = result ? output : (decimal?)null;
            return result;
        }

        public static bool TryParse(string input, CultureInfo culture, out double? nullableOutput)
        {
            if (input == null)
            {
                nullableOutput = null;
                return true;
            }

            var stringValue = input
                .Replace(".", culture.NumberFormat.NumberDecimalSeparator)
                .Replace(",", culture.NumberFormat.NumberDecimalSeparator);
            var result = double.TryParse(stringValue, NumberStyles.Any, culture, out var output);
            nullableOutput = result ? output : (double?)null;
            return result;
        }
    }
}
