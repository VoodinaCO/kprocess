using System;

namespace KProcess.Ksmed.Models
{
    /// <summary>
    /// Identifie le comportement d'une entité qui peut être auditée.
    /// </summary>
    public interface IAuditable
    {

        /// <summary>
        /// Obtient ou définit la date de création de l'entité.
        /// </summary>
        DateTime CreationDate { get; set; }

        /// <summary>
        /// Obtient ou définit la dernière date de modification.
        /// </summary>
        DateTime LastModificationDate { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'audit ne doit pas être géré automatiquement.
        /// </summary>
        bool CustomAudit { get; }
    }
}
