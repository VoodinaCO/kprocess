using KProcess.Globalization;

namespace KProcess.Ksmed.Models
{
    /// <summary>
    /// Fournit des méthodes d'aide à la localisatin.
    /// </summary>
    public static class LocalizationHelpers
    {
        /// <summary>
        /// Obtient le nom complet d'une personne de manière localisée.
        /// </summary>
        /// <param name="firstName">Le prénom.</param>
        /// <param name="lastName">le nom.</param>
        /// <returns></returns>
        public static string GetLocalizedFullName(string firstName, string lastName)
        {
            //string loc = IoC.Resolve<ILocalizationManager>().GetString("Common_UserFullName");
            string loc = "";
            if (!string.IsNullOrEmpty(loc))
                return string.Format(
                    loc,
                    firstName ?? string.Empty,
                    lastName ?? string.Empty);
            return $"{(firstName ?? string.Empty)} {(lastName ?? string.Empty)}";
        }
    }
}
