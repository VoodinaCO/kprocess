using System;

namespace KProcess.Ksmed.Models
{
    partial class QualificationReason : ICloneable
    {
        public override string ToString()
        {
            return Comment;
        }

        public object Clone() =>
            MemberwiseClone();
    }
}
