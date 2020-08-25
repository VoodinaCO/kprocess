using KProcess.KL2.Languages.Model;
using System.Collections.Generic;

namespace KProcess.KL2.Languages.Provider
{
    public interface ILanguageStorageProvider
    {
        /// <summary>
        /// Load from the provider the localized string as a dictionnary
        /// Key = Culture
        /// Value = Dictionnary with key = string key and value = string value
        /// </summary>
        /// <returns></returns>
        IDictionary<string, IDictionary<string, string>> LoadLocalizedStrings();

        void CreateDatabase();

        void Save(IDictionary<string, IList<LocalizedStringValue>> values);
    }
}
