using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.KL2.SetupUI
{
    public static class LanguagesExt
    {
        public static string ToCultureInfoString(this Languages lang)
        {
            switch (lang)
            {
                case Languages.Français:
                    return "fr-FR";
                default:
                case Languages.English:
                    return "en-US";
            }
        }
        public static Languages ToLanguages(this CultureInfo culture)
        {
            var cultureString = culture.ToString();
            if (cultureString == "fr-FR") return Languages.Français;
            else if (cultureString == "en-US") return Languages.English;
            else return Languages.English;
        }
    }
}
