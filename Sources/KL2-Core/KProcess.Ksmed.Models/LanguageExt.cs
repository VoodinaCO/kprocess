using System;

namespace KProcess.Ksmed.Models
{
    partial class Language : IEquatable<Language>
    {
        #region IEquatable<Language> Members

        public bool Equals(Language other) =>
            other != null && string.Equals(LanguageCode, other.LanguageCode);

        public override bool Equals(object obj) =>
            Equals(obj as Language);

        public override int GetHashCode() =>
            LanguageCode != null ? LanguageCode.GetHashCode() : base.GetHashCode();

        #endregion
    }
}
