using KProcess.KL2.Languages.Provider;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace KProcess.KL2.Languages
{
    public class LocalizedStrings : ILocalizedStrings
    {
        private readonly ILanguageStorageProvider _languageStoreProvider;
        private const string defaultLanguage = "fr-FR";
        private IDictionary<string, IDictionary<string, string>> _localizedStrings;

        /// <summary>
        /// Constructor with provider to retrieve the localized label in parameter
        /// </summary>
        public LocalizedStrings(ILanguageStorageProvider languageStorageProvider)
        {
            _languageStoreProvider = languageStorageProvider;
            Reload();
        }

        public List<CultureInfo> GetSupportedLanguages() =>
            _localizedStrings.Keys.Select(_ => new CultureInfo(_)).ToList();

        public IEnumerable<string> GetAllKeys()
        {
            return _localizedStrings.First().Value.Keys;
        }

        /// <summary>
        /// Reload dictionnary of localized strings
        /// </summary>
        public void Reload()
        {
            _localizedStrings = _languageStoreProvider.LoadLocalizedStrings();
        }

        /// <summary>
        /// Retrieve value information for a specific key and language
        /// </summary>
        /// <param name="key"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public string GetLanguageValue(string key, string language)
        {
            if (string.IsNullOrEmpty(language) || !_localizedStrings.ContainsKey(language))
                language = defaultLanguage;
            if (!_localizedStrings.ContainsKey(language))
                return string.Empty;
            if (!_localizedStrings[language].ContainsKey(key))
                return string.Empty;
            else
                return _localizedStrings[language][key];
        }

        /// <summary>
        /// Retrieve value information for a specific key and language with parameters
        /// </summary>
        /// <param name="key"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public string GetLanguageValueFormat(string key, string language, params string[] parameters)
        {
            var localizedValue = GetLanguageValue(key, language);
            if (string.IsNullOrEmpty(localizedValue) || parameters == null || parameters.Length == 0)
                return localizedValue;
            return string.Format(localizedValue, parameters);
        }
    }
}
