using System;

namespace KProcess.Ksmed.Models
{
    partial class Objective : IEquatable<Objective>
    {
        #region Propriétés de présentation

        #endregion

        #region IEquatable<Objective> Members

        public bool Equals(Objective other) =>
            other != null && string.Equals(ObjectiveCode, other.ObjectiveCode);

        public override bool Equals(object obj) =>
            Equals(obj as Objective);

        public override int GetHashCode() =>
            ObjectiveCode != null ? ObjectiveCode.GetHashCode() : 0;

        #endregion
    }
}
