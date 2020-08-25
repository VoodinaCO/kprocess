﻿namespace KProcess.Ksmed.Models
{
    /// <summary>
    /// Identifie le comportement d'une entité qui peut être auditée et qui ne requiert pas des références vers les utilisateurs.
    /// </summary>
    public interface IAuditableUserOptional : IAuditable
    {

        /// <summary>
        /// Obtient ou définit l'identifiant de l'utilisateur qui a créé l'entité.
        /// </summary>
        int? CreatedByUserId { get; set; }

        /// <summary>
        /// Obtient ou définit l'identifiant de l'utilisateur qui a modifié l'entité.
        /// </summary>
        int? ModifiedByUserId { get; set; }
    }
}
