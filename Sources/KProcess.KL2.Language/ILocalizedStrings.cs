using System.Collections.Generic;
using System.Globalization;

namespace KProcess.KL2.Languages
{
    public interface ILocalizedStrings
    {
        void Reload();
        List<CultureInfo> GetSupportedLanguages();
        IEnumerable<string> GetAllKeys();
        string GetLanguageValue(string key, string language);
        string GetLanguageValueFormat(string key, string language, params string[] parameters);
    }
}
